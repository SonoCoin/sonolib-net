using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Sonolib.Contracts.Event
{
    public class ContractEventCall
    {
        public ContractEvent Event { get; set; }
        public ICollection<ContractEventArgCall> ArgsCall { get; set; }

        public ContractEventCall(ContractEvent function)
        {
            Event = function;
            ArgsCall = new List<ContractEventArgCall>();
        }

        public string GetArgsJson()
        {
            var args = ArgsCall.Select(argCall => argCall.Value).ToList();

            return args.Count > 0 ? JsonSerializer.Serialize(args) : null;
        }
    }
}