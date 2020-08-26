using System;
using System.Collections.Generic;
using SimpleBase;
using Sonolib.Extensions;

namespace Sonolib.Dtos
{
    public class ContractMessageDto
    {
        public string Sender { get; set; }
        public string Address { get; set; }
        public string Payload { get; set; }
        public ulong Value { get; set; }
        
        // requested commission
        public ulong Gas { get; set; }
        
        public IEnumerable<byte> ToBytes()
        {
            var payload = new List<byte>();
            
            // Step 1: add Sender (26 bytes)
            payload.AddRange(Base58.Bitcoin.Decode(Sender).ToArray());
            
            // Step 2: add Payload
            payload.AddRange(Payload.HexDecode());
                
            // Step 3 add Value (8 bytes)
            payload.AddRange(BitConverter.GetBytes(Value));
            
            // Step 4 add Value (8 bytes)
            payload.AddRange(BitConverter.GetBytes(Gas));

            if (Address.HasValue())
            {
                // Step 5: add Address (26 bytes)
                payload.AddRange(Base58.Bitcoin.Decode(Address).ToArray());
            }

            return payload;
        }
    }
}