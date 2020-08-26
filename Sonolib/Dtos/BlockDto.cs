using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sonolib.Dtos.Extended;

namespace Sonolib.Dtos
{
    public class BlockDto
    {
        public BlockHeaderDto Header { get; set; }
        
        [JsonPropertyName("txs")]
        public List<TransactionWithLogsDto> Transactions { get; set; }
        
        public List<AdviceDto> Advices { get; set; }
    }
}