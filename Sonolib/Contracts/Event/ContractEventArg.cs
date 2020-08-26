﻿using Sonolib.Contracts.Functions;

 namespace Sonolib.Contracts.Event
{
    public class ContractEventArg
    {
        public ContractFunctionArg Arg { get; set; }
        public int Order { get; set; }
        public ContractEventArgType Type { get; set; }

        public ContractEventArg(ContractEventArgType type, int order, ContractFunctionArg arg)
        {
            Arg = arg;
            Order = order;
            Type = type;
        }
    }
}