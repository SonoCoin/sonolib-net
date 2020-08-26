using System;
using System.Collections.Generic;
using Sonolib.Extensions;
using SimpleBase;

namespace Sonolib.Dtos
{
    public class TransactionInputDto
    {
        public string Address { get; set; }
        public ulong Nonce { get; set; }
        public string Sign { get; set; }
        
        public string PublicKey { get; set; }
        public ulong Value { get; set; }

        public IEnumerable<byte> ToBytes()
        {
            var payload = new List<byte>();

            // Step 1: add Address (26 bytes)
            payload.AddRange(Base58.Bitcoin.Decode(Address).ToArray());
            
            // Step 2 add Value (8 bytes)
            payload.AddRange(BitConverter.GetBytes(Value));

            // Step 3 add Nonce (8 bytes)
            payload.AddRange(BitConverter.GetBytes(Nonce));

            // Different payload for sign and for hash,
            // for sign we use without sign and public key fields
            // for tx hash we use with sign and public key
            if (Sign.HasValue() && PublicKey.HasValue())
            {
                // Step 5 add Nonce (8 bytes)
                payload.AddRange(Sign.HexDecode());
            
                // Step 6 add Nonce (8 bytes)
                payload.AddRange(PublicKey.HexDecode());
            }

            return payload;
        }
    }
}