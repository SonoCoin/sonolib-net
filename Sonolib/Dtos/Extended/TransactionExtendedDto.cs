namespace Sonolib.Dtos.Extended
{
    public class TransactionExtendedDto : TransactionWithLogsDto
    {
        public int Size { get; set; }
        public string Block { get; set; }
        public int Height { get; set; }
        public int Confirmed { get; set; }
        public int ConfirmedTimestamp { get; set; }
    }
}