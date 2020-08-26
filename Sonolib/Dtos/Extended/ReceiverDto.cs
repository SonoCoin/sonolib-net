using NBitcoin;

namespace Sonolib.Dtos.Extended
{
    public class ReceiverDto
    {
        public string Address { get; set; } 
        public decimal Amount { get; set; }

        public ulong AmountUlong => (ulong) Amount * (ulong) MoneyUnit.BTC;
    }
}