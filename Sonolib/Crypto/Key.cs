using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Sodium;
using Sonolib.Extensions;
using Sonolib.Helpers;

namespace Sonolib.Crypto
{
    public class Key
    {
        private const string Ed25519Curve = "Sonocoin seed";
        private const uint HardenedOffset = 0x80000000;
        
        /// <summary>
        /// 32 length
        /// </summary>
        public byte[] PrivateKey { get; private set; }
        
        /// <summary>
        /// 32 length
        /// </summary>
        public byte[] PublicKey { get; private set; }
        
        private KeyPair KeyPair { get; set; }
        
        public Key(byte[] seed, int index)
        {
            var encoder = new ASCIIEncoder();
            var secret = encoder.DecodeData(Ed25519Curve);

            // var seedBytes = HexByteConvertorExtensions.HexToByteArrayInternal(seed);
            var array = new HMACSHA512(secret).ComputeHash(seed);

            var key = array.SafeSubarray(0, 32);
            var chainCode = array.SafeSubarray(32, 32);

            var derivedPath = unchecked((uint) index+HardenedOffset);
            derivedPath = HexByteConvertorExtensions.ReverseBytes(derivedPath);
            var dpBytes = BitConverter.GetBytes(derivedPath);
            var data = new byte[]{0}.Concat(key, dpBytes);
            
            var array2 = new HMACSHA512(chainCode).ComputeHash(data);
            
            var key2 = array2.SafeSubarray(0, 32);

            var keyPair = PublicKeyAuth.GenerateKeyPair(key2);
            PrivateKey = keyPair.PrivateKey;
            PublicKey = keyPair.PublicKey;
        }

        public Key(byte[] privateKey)
        {
            var keyPair = PublicKeyAuth.GenerateKeyPair(privateKey);
            PrivateKey = keyPair.PrivateKey;
            PublicKey = keyPair.PublicKey;
        }
        
        public Key(byte[] privateKey, byte[] publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
        
        public Wallet ToWallet()
        {
            return new Wallet(PublicKey);
        }
        
        public static IEnumerable<byte> Sign(byte[] msg, byte[] privateKey)
        {
            return PublicKeyAuth.SignDetached(msg, privateKey);
        }

        public static bool Verify(byte[] sign, byte[] msg, byte[] publicKey)
        {
            return PublicKeyAuth.VerifyDetached(sign, msg, publicKey);
        }
    }
}