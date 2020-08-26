using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sonolib.Contracts.Functions
{
    public class ContractFunctionCall
    {
        public ContractFunction Function { get; set; }
        public ICollection<ContractFunctionArgCall> ArgsCall { get; set; }

        public ContractFunctionCall(ContractFunction function)
        {
            Function = function;
            ArgsCall = new List<ContractFunctionArgCall>();
        }
        
        public ContractFunctionCall(ContractFunction function, ICollection<ContractFunctionArgCall> argsCall)
        {
            Function = function;
            ArgsCall = argsCall;
        }
        
        public string GetArgsJson()
        {
            var args = ArgsCall.Select(argCall => argCall.Value).ToList();

            return args.Count > 0 ? JsonSerializer.Serialize(args) : null;
        }
    }
}