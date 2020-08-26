namespace Sonolib.Dtos.Extended
{
    public class TransactionResultDto
    {
        /// <summary>
        /// Ok - if success
        /// </summary>
        public string Result { get; set; }
        
        /// <summary>
        /// Transaction Id
        /// </summary>
        public string TxId { get; set; }
    }
}