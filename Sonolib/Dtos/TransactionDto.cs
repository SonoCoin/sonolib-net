using System.Collections.Generic;

namespace Sonolib.Dtos
{
    public class TransactionDto
    {
        public TransactionRequest Request { get; set; }
        public ICollection<TransactionDeltaDto> Incomes { get; set; }
        public ICollection<TransactionDeltaDto> Outcomes { get; set; }
        public ICollection<State> States { get; set; }
    }
}
