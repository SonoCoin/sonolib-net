using System.Collections.Generic;

namespace Sonolib.Dtos.Extended
{
    /// <summary>
    /// Transactions hash list response
    /// </summary>
    public class TxRequest
    {
        public string NetworkName { get; set; } = "Main";

        public List<string> HashList { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class TxRequestByHash
    {
        public List<string> HashList { get; set; }
    }
}