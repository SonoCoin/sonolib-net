using System.Collections.Generic;

namespace Sonolib.Dtos.Extended
{
    /// <summary>
    /// Balancess request model
    /// </summary>
    public class BalancesRequest
    {
        public List<string> Addresses { get; set; }
    }
    
    public class BalanceDto
    {
        public string Address { get; set; }
        
        public decimal ConfirmedAmount { get; set; }
        public decimal UnconfirmedAmount { get; set; }
    }

    public class BalanceListDto
    {
        public ICollection<BalanceDto> Balances { get; set; }
    }
    
}