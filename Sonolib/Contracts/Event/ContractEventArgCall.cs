﻿namespace Sonolib.Contracts.Event
{
    public class ContractEventArgCall
    {
        public ContractEventArg Arg { get; set; }
        public string Value { get; set; }

        public ContractEventArgCall(ContractEventArg arg, string val)
        {
            Arg = arg;
            Value = val;
        }
    }
}