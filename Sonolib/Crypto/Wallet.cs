using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Sonolib.Extensions;
using Nethereum.HdWallet;
using SimpleBase;

namespace Sonolib.Crypto
{
    public class Wallet
    {
        private const int AddressChecksumLen = 4;
        public readonly string Base58Address;
        
        private static readonly byte[] WalletVersion =
        {
            14, // S
            48, // C
        };
        
        public static bool IsValidAddress(string address)
        {
            byte[] addressBytes;
            try
            {
                addressBytes = Base58.Bitcoin.Decode(address).ToArray();
            }
            catch
            {
                return false;
            }
            
            if (addressBytes.Length < WalletVersion.Length + AddressChecksumLen) {
                return false;
            }
            
            var ver = addressBytes.SafeSubarray(0, WalletVersion.Length);
            if (ver.ToHex() != WalletVersion.ToHex())
            {
                return false;
            }
            
            var payload = addressBytes.SafeSubarray(0, addressBytes.Length - AddressChecksumLen);
            var checksum = Checksum(payload.ToArray());
            var checkAddress = payload.Concat(checksum);

            return checkAddress.ToHex() == addressBytes.ToHex();
        }
        
        public Wallet(byte[] publicKey)
        {
            // Step 1: Allocate payload list
            var payload = new List<byte>();

            // Step 2: Append version
            //payload.Add(version);
            payload.AddRange(WalletVersion);

            // Step 3: Create ripmd160(sha256(publicKey)) hash
            var pub256Key = SHA256.Create().ComputeHash(publicKey);
            var ripmd160 = RIPEMD160.Create().ComputeHash(pub256Key);

            // Step 4: append hash to payload
            payload.AddRange(ripmd160);

            // Step 5: compute checksum of current payload
            var checksum = Checksum(payload.ToArray());

            // Step 6: append computed checksum to payload
            payload.AddRange(checksum);

            var address = payload.ToArray();

            // Step 7: convert payload bytes to base58 format
            Base58Address = Base58.Bitcoin.Encode(address);
        }

        private static byte[] Checksum(byte[] payload)
        {
            var firstSha256 = SHA256.Create().ComputeHash(payload);
            var secondSha256 = SHA256.Create().ComputeHash(firstSha256);
            var checksum = new byte[AddressChecksumLen];
            Array.Copy(secondSha256, checksum, AddressChecksumLen);
            return checksum;
        }
    }
}