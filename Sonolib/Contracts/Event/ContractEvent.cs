using System.Collections.Generic;
using Sonolib.Contracts.Functions;

namespace Sonolib.Contracts.Event
{
    public class ContractEvent
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        
        public ICollection<ContractEventArg> Args { get; set; }

        public ContractEvent(string name, string hex, ICollection<ContractEventArg> args)
        {
            Name = name;
            Hex = hex;
            Args = args;
        }
        
        public static ContractEvent TRANSFER = new ContractEvent(
            "TRANSFER_EVENT", 
            "831ac82b07fb396dafef0077cea6e002235d88e63f35cbd5df2c065107f1e74a",
            new List<ContractEventArg>
            {
                new ContractEventArg(ContractEventArgType.Name, 0, ContractFunctionArg.Name),
                new ContractEventArg(ContractEventArgType.FromAddress, 1, ContractFunctionArg.Address),
                new ContractEventArg(ContractEventArgType.ToAddress, 2, ContractFunctionArg.Address),
                new ContractEventArg(ContractEventArgType.Amount, 3, ContractFunctionArg.Uint64),
            }); 
        
        public static ContractEvent APPROVAL = new ContractEvent(
            "APPROVAL_EVENT", 
            "16304dfea7f3fbabcf59225f0629cb307fecb8d5652b069080aa9be2c765d7d2", 
            new List<ContractEventArg>
            {
                new ContractEventArg(ContractEventArgType.Name, 0, ContractFunctionArg.Name),
                new ContractEventArg(ContractEventArgType.FromAddress, 1, ContractFunctionArg.Address),
                new ContractEventArg(ContractEventArgType.ToAddress, 2, ContractFunctionArg.Address),
                new ContractEventArg(ContractEventArgType.Amount, 3, ContractFunctionArg.Uint64),
            }); 
    }
}