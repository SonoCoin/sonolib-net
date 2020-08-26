﻿using Sonolib.Contracts.Extensions;

 namespace Sonolib.Contracts.Functions
{
    public class ContractFunctionArgCall
    {
        public ContractFunctionArg Arg { get; set; }
        public string Value { get; set; }

        public ContractFunctionArgCall(ContractFunctionArg arg, string val)
        {
            Arg = arg;
            Value = val;
        }

        public string ToPayloadHex()
        {
            return Value.ToPayloadHex(Arg.Type);
        }
    }
}