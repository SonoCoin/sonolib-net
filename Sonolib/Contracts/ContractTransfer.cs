﻿﻿namespace Sonolib.Contracts
{
    public class ContractTransfer
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public ulong Amount { get; set; }
    }
}