using System.Collections.Generic;
using Sonolib.Extensions;

namespace Sonolib.Dtos
{
    public class StakeDto : TransferDto
    {
        public string NodeId { get; set; }

        public new IEnumerable<byte> ToBytes()
        {
            var payload = base.ToBytes();

            if (NodeId.HasValue())
            {
                payload.AddRange(NodeId.HexDecode());
            }

            return payload;
        }
    }
}