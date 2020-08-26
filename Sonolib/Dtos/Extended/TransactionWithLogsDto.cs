using System.Collections.Generic;

namespace Sonolib.Dtos.Extended
{
    public class TransactionWithLogsDto : TransactionDto
    {
        public ICollection<VmDataDto> VmData { get; set; }
    }
}