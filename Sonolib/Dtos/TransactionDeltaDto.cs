using System;
using System.Collections.Generic;
using SimpleBase;

namespace Sonolib.Dtos
{
    public class TransactionDeltaDto
    {
        public TxObjectType Type { get; set; }
        public int ParentIndex { get; set; }
        public ulong Value { get; set; }
        public string Address { get; set; }

        public List<byte> ToBytes()
        {
            var payload = new List<byte>();
            
            // Step 1: add Address (26 bytes)
            payload.AddRange(Base58.Bitcoin.Decode(Address).ToArray());
            
            // Step 2: add Type (4 bytes)
            payload.AddRange(BitConverter.GetBytes((int) Type));
            
            // Step 3 add Value (4 bytes)
            payload.AddRange(BitConverter.GetBytes(ParentIndex));
            
            // Step 4 add Value (4 bytes)
            payload.AddRange(BitConverter.GetBytes(Value));
            
            return payload;
        }
    }
}