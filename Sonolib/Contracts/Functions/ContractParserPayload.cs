using System;
using System.Collections.Generic;
using System.Linq;
using SimpleBase;
using Sonolib.Contracts.Extended;
using Sonolib.Extensions;

namespace Sonolib.Contracts.Functions
{
    public static class ContractParserPayload
    {
        private static readonly ICollection<ContractType> TokenTypes = new List<ContractType>
        {
            ContractType.Erc20
        };

        public static bool IsToken(this ContractType type)
        {
            return TokenTypes.Contains(type);
        }

        public static ContractType GetContractType(this string payload)
        {
            try
            {
                var types = (ContractType[]) Enum.GetValues(typeof(ContractType));
                foreach (var type in types)
                {
                    if (payload.IsContractType(type))
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

        // @TODO maybe rewrite by regex 
        public static bool IsContractType(this string payload, ContractType type)
        {
            var functions = type switch
            {
                ContractType.Erc20 => Erc20.Functions,
                ContractType.Unknown => throw new Exception("ContractParserPayload:IsContractType error: unknown type"),
                _ => throw new Exception("ContractParserPayload:IsContractType error: invalid type")
            };
            
            var p = payload;
            foreach (var func in functions)
            {
                var str = func.NameHex;

                if (!p.Contains(str))
                {
                    return false;
                }
                p = p.Replace(str, "");
            }

            return true;
        }

        public static ContractFunctionCall GetFunctionCall(this string payload, ContractType type)
        {
            var func = payload.Substring(0, 8);
            ICollection<ContractFunction> contractFunctions = null;
            
            switch (type)
            {
                case ContractType.Erc20:
                    contractFunctions = Erc20.Functions;
                    break;
                case ContractType.Unknown:
                    break;
                default:
                    throw new Exception("ContractParserPayload:GetFunctionCall err: invalid type");
            }

            ContractFunctionCall call = null;
            foreach (var f in contractFunctions)
            {
                if (f.NameHex == func)
                {
                    call = new ContractFunctionCall(f);
                    break;
                }
            }

            if (call == null)
            {
                throw new Exception($"ContractParserPayload:GetFunctionCall invalid payload for this type:{type}");
            }
            
            var argsPayload = payload.Substring(8);

            var prevIndex = 0;
            foreach (var arg in call.Function.Args)
            {
                var len = arg.Length * 2;
                var hex = argsPayload.Substring(prevIndex, len);
                var buffer = hex.HexDecode();

                var val = arg.Type switch
                {
                    ContractFunctionArgType.Address => Base58.Bitcoin.Encode(buffer),
                    ContractFunctionArgType.Uint64 => BitConverter.ToUInt64(buffer.Reverse().ToArray(), 0).ToString(),
                    _ => throw new Exception($"Invalid arg type: type {type}, arg: {arg.Type}")
                };

                call.ArgsCall.Add(new ContractFunctionArgCall(arg, val));
                
                prevIndex += len;
            }
            
            return call;
        }

        public static ContractTransfer GetTransfer(this string payload)
        {
            var type = payload.GetContractType();
            switch (type)
            {
                case ContractType.Erc20:
                    var call = payload.GetFunctionCall(ContractType.Erc20);
                    switch (call.Function.Name)
                    {
                        case "transfer":
                            var toAddress = "";
                            var amount = (ulong) 0;
                            foreach (var argCall in call.ArgsCall.OrderBy(x => x.Arg.Order))
                            {
                                switch (argCall.Arg.Type)
                                {
                                    case ContractFunctionArgType.Address:
                                        toAddress = argCall.Value;
                                        break;
                                    case ContractFunctionArgType.Uint64:
                                        amount = ulong.Parse(argCall.Value);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException($"ContractParserPayload:GetTransfer Invalid arg type: {argCall.Arg.Type}");
                                }
                            }
                            
                            return new ContractTransfer
                            {
                                FromAddress = null,
                                ToAddress = toAddress,
                                Amount = amount,
                            };
                        default:
                            return null;
                    }
                case ContractType.Unknown:
                    return null;
                default:
                    return null;
                    // throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        } 
        
        public static ContractTransfer GetErc20Transfer(this string payload)
        {
            var call = payload.GetFunctionCall(ContractType.Erc20);
            switch (call.Function.Name)
            {
                case "transfer":
                    var transferToAddress = "";
                    var transferAmount = (ulong) 0;
                    foreach (var argCall in call.ArgsCall.OrderBy(x => x.Arg.Order))
                    {
                        switch (argCall.Arg.Type)
                        {
                            case ContractFunctionArgType.Address:
                                transferToAddress = argCall.Value;
                                break;
                            case ContractFunctionArgType.Uint64:
                                transferAmount = ulong.Parse(argCall.Value);
                                break;
                            case ContractFunctionArgType.Unknown:
                                break;
                            default:
                                return null;
                                throw new ArgumentOutOfRangeException($"ContractParserPayload:GetTransfer Invalid arg type: {argCall.Arg.Type}");
                        }
                    }
                    
                    return new ContractTransfer
                    {
                        FromAddress = null,
                        ToAddress = transferToAddress,
                        Amount = transferAmount,
                    };
                
                case "transferFrom":
                    var fromAddress = "";
                    var toAddress = "";
                    var amount = (ulong) 0;
                    foreach (var argCall in call.ArgsCall.OrderBy(x => x.Arg.Order))
                    {
                        switch (argCall.Arg.Type)
                        {
                            case ContractFunctionArgType.Address:
                                if (argCall.Arg.Order == 0)
                                {
                                    fromAddress = argCall.Value;   
                                }
                                else
                                {
                                    toAddress = argCall.Value;
                                }
                                break;
                            case ContractFunctionArgType.Uint64:
                                amount = ulong.Parse(argCall.Value);
                                break;
                            case ContractFunctionArgType.Unknown:
                                break;
                            case ContractFunctionArgType.String:
                                break;
                            default:
                                return null;
                                throw new ArgumentOutOfRangeException($"ContractParserPayload:GetTransfer Invalid arg type: {argCall.Arg.Type}");
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