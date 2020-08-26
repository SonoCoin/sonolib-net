using System;
using System.Collections.Generic;
using System.Linq;
using SimpleBase;
using Sonolib.Contracts.Extended;
using Sonolib.Dtos.Extended;
using Sonolib.Extensions;

namespace Sonolib.Contracts.Event
{
    public static class ContractEventParser
    {
        private const int HexLen = 32;
        private const string Empty = "0000000000000000000000000000000000000000000000000000000000000000";

        public static ContractType GetContractType(this VmDataDto data)
        {
            try
            {
                var types = (ContractType[]) Enum.GetValues(typeof(ContractType));
                foreach (var type in types)
                {
                    if (type == ContractType.Unknown) continue;
                    if (data.IsContractType(type))
                    {
                        return type;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return ContractType.Unknown;
        }
        
        public static bool IsContractType(this VmDataDto data, ContractType type)
        {
            var logs = data.Logs.FirstOrDefault();
            if (logs == null)
            {
                throw new Exception("ContractParser:IsContractType error: logs are empty");
            }
            
            var eventCreate = type switch
            {
                ContractType.Erc20 => Erc20.FirstEvent,
                ContractType.Unknown => throw new Exception("ContractParser:Is err: unknown type"),
                _ => throw new Exception("ContractParser:Is err: invalid type")
            };

            var firstTopic = logs.Topics.FirstOrDefault();
            if (firstTopic == null)
            {
                throw new Exception("ContractParser:IsContractType error: topics are empty");
            }

            return firstTopic == eventCreate.Hex;
        }
        
        
        public static ContractEventCall GetEventCall(this VmDataDto data, ContractType type)
        {
            var logs = data.Logs.FirstOrDefault();
            if (logs == null)
            {
                throw new Exception("ContractParser:IsContractType error: logs are empty");
            }
            
            // var type = data.GetContractType();
            
            var func = logs.Topics.FirstOrDefault();
            if (!func.HasValue())
            {
                throw new Exception($"ContractEventParser:GetEventCall data has 0 topics");
            }
            
            ICollection<ContractEvent> contractEvents = null;
            
            switch (type)
            {
                case ContractType.Erc20:
                    contractEvents = Erc20.Events.ToList();
                    break;
                case ContractType.Unknown:
                    break;
                default:
                    throw new Exception("ContractEventParser:GetEventCall err: invalid type");
            }

            ContractEventCall call = null;
            
            foreach (var ev in contractEvents)
            {
                if (ev.Hex == func)
                {
                    call = new ContractEventCall(ev);
                    break;
                }
            }
        
            if (call == null)
            {
                throw new Exception($"ContractEventParser:GetEventCall invalid topic for this type:{type}");
            }

            var topics = logs.Topics.ToList();
            foreach (var arg in call.Event.Args)
            {
                var skip = HexLen - arg.Arg.Length; 
                var topic = topics[arg.Order];
                var buffer = topic
                    .Substring(skip * 2) // multiple 2 because of hex
                    .HexDecode(); 
        
                var val = arg.Type switch
                {
                    ContractEventArgType.Name => buffer.ToHex(),
                    ContractEventArgType.FromAddress => topic == Empty ? null : Base58.Bitcoin.Encode(buffer),
                    ContractEventArgType.Amount => BitConverter.ToUInt64(buffer.Reverse().ToArray(), 0).ToString(),
                    ContractEventArgType.ToAddress => Base58.Bitcoin.Encode(buffer),
                    _ => throw new Exception($"ContractEventParser:GetEventCall invalid arg type: type {type}, arg: {arg.Type}")
                };
        
                call.ArgsCall.Add(new ContractEventArgCall(arg, val));
            }
            
            return call;
        }
        
        public static ContractTransfer GetErc20Transfer(this VmDataDto data)
        {
            var call = data.GetEventCall(ContractType.Erc20);
            switch (call.Event.Name)
            {
                case "TRANSFER_EVENT":
                    var fromAddress = "";
                    var toAddress = "";
                    var amount = (ulong) 0;
                    foreach (var argCall in call.ArgsCall.OrderBy(x => x.Arg.Order))
                    {
                        switch (argCall.Arg.Type)
                        {
                            case ContractEventArgType.FromAddress:
                                fromAddress = argCall.Value;
                                break;
                            case ContractEventArgType.ToAddress:
                                toAddress = argCall.Value;
                                break;
                            case ContractEventArgType.Amount:
                                amount = ulong.Parse(argCall.Value);
                                break;
                            case ContractEventArgType.Name:
                                break;
                            default:
                                return null;
                        }
                    }
                    
                    return new ContractTransfer
                    {
                        FromAddress = fromAddress,
                        ToAddress = toAddress,
                        Amount = amount,
                    };
                default:
                    return null;
            }
        } 
    }
}