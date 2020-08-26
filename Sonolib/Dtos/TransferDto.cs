using System;
using System.Collections.Generic;
using SimpleBase;

namespace Sonolib.Dtos
{
    public class TransferDto
    {
        public string Address { get; set; }
        public ulong Value { get; set; }
        
        public List<byte> ToBytes()
        {
            var payload = new List<byte>();
            
            // Step 1: add Address
            payload.AddRange(Base58.Bitcoin.Decode(Address).ToArray());
            
            // Step 2 add Value (8 bytes)
            payload.AddRange(BitConverter.GetBytes(Value));
            
            return payload;
        }
    }
}