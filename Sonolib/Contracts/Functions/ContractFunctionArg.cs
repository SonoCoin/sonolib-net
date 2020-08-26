﻿using System.Text.Json.Serialization;
 
 namespace Sonolib.Contracts.Functions
{
    public class ContractFunctionArg
    {
        public string Title { get; set; }
        
        [JsonIgnore]
        public ContractFunctionArgType Type { get; set; }

        [JsonPropertyName("type")]
        public string TypeString => Type.ToString();
        public int Order { get; set; }

        public int Length
        {
            get
            {
                return Type switch
                {
                    ContractFunctionArgType.Address => 26,
                    ContractFunctionArgType.Uint64 => 8,
                    _ => 32
                };
            }
        }

        public ContractFunctionArg(string title, ContractFunctionArgType type)
        {
            Title = title;
            Type = type;
        }

        public static readonly ContractFunctionArg Name = new ContractFunctionArg("Name", ContractFunctionArgType.Unknown);
        public static readonly ContractFunctionArg Address = new ContractFunctionArg("Address", ContractFunctionArgType.Address);
        public static readonly ContractFunctionArg Uint64 = new ContractFunctionArg("Amount", ContractFunctionArgType.Uint64);
    }
}