using System.Collections.Generic;
using Sonolib.Contracts.Event;

namespace Sonolib.Contracts.Functions
{
    public class ContractFunction
    {
        public string Name { get; set; }
        public string NameHex { get; set; }
        public string DisplayName { get; set; }
        public bool IsStatic { get; set; }
        
        public ContractEvent Event { get; set; }

        // make ordered collection
        public IEnumerable<ContractFunctionArg> Args { get; set; }

        public ContractFunction(string name, string nameHex, string displayName, bool isStatic, IEnumerable<ContractFunctionArg> args = null, ContractEvent contractEvent = null)
        {
            Name = name;
            NameHex = nameHex;
            DisplayName = displayName;
            IsStatic = isStatic;
            Args = args ?? new List<ContractFunctionArg>();
            Event = contractEvent;
        }
    }
}