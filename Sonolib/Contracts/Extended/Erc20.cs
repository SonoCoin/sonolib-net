using System.Collections.Generic;
using System.Linq;
using Sonolib.Contracts.Event;
using Sonolib.Contracts.Functions;

namespace Sonolib.Contracts.Extended
{
    public static class Erc20
    {
        public static readonly ContractEvent FirstEvent = ContractEvent.TRANSFER;

        public static readonly ContractFunction BalanceOf =
            new ContractFunction("balanceOf", "70a08231", "balanceOf(address)", true, new List<ContractFunctionArg>
            {
                ContractFunctionArg.Address,
            });

        public static readonly ContractFunction Transfer =
            new ContractFunction("transfer", "5d359fbd", "transfer(address,uint64)", false, new List<ContractFunctionArg>
            {
                ContractFunctionArg.Address,
                ContractFunctionArg.Uint64,
            }, ContractEvent.TRANSFER);

        public static readonly ContractFunction Name = new ContractFunction("name", "06fdde03", "name()", true);
        public static readonly ContractFunction Symbol = new ContractFunction("symbol", "95d89b41", "symbol()", true);
        public static readonly ContractFunction Decimals = new ContractFunction("decimals", "313ce567", "decimals()", true);
        public static readonly ContractFunction TotalSupply =
            new ContractFunction("totalSupply", "18160ddd", "totalSupply()", true);


        public static readonly ContractFunction Approve =
            new ContractFunction("approve", "1086a9aa", "approve(address,uint64)", false, new List<ContractFunctionArg>
            {
                ContractFunctionArg.Address,
                ContractFunctionArg.Uint64,
            }, ContractEvent.APPROVAL);

        public static readonly ContractFunction Allowance =
            new ContractFunction("allowance", "dd62ed3e", "allowance(address,address)", true, new List<ContractFunctionArg>
            {
                ContractFunctionArg.Address,
                ContractFunctionArg.Address,
            });

        public static readonly ContractFunction TransferFrom =
            new ContractFunction("transferFrom", "2ea0dfe1", "transferFrom(address,address,uint64)", false,
                new List<ContractFunctionArg>
                {
                    ContractFunctionArg.Address,
                    ContractFunctionArg.Address,
                    ContractFunctionArg.Uint64,
                });
        
        public static IEnumerable<ContractEvent> Events => Functions
            .Where(x => x.Event != null)
            .Select(x => x.Event);

        // @TODO move to parser file: maybe func(params) on a new line or move to database
        public static readonly ICollection<ContractFunction> Functions = new List<ContractFunction>
        {
            BalanceOf,
            Transfer,
            Name,
            Symbol,
            Decimals,
            TotalSupply,
            Approve,
            Allowance,
            TransferFrom,
        };
    }
}