using System;
using System.Linq;
using System.Text;
using SimpleBase;
using Sonolib.Contracts.Functions;
using Sonolib.Extensions;

namespace Sonolib.Contracts.Extensions
{
    public static class TypesExtensions
    {
        public static T ConvertTo<T>(this string value)
        {
            try
            {
                object tmp = null;
                var buffer = value.HexDecode();
                if (typeof(T) == typeof(ulong))
                {
                    tmp = BitConverter.ToUInt64(buffer.Reverse().ToArray(), 0).ToString();
                } else if (typeof(T) == typeof(byte))
                {
                    tmp = buffer.FirstOrDefault();
                } else if (typeof(T) == typeof(string))
                {
                    tmp = Encoding.ASCII.GetString(buffer);
                }
                else
                {
                    throw new Exception($"TypesExtensions:ConvertTo error: invalid T {typeof(T)}");
                }

                return (T)Convert.ChangeType(tmp, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"TypesExtensions:ConvertTo error: {ex.Message}");
            }
        }

        public static string ToPayloadHex(this object obj, ContractFunctionArgType type)
        {
            var buf = type switch
            {
                ContractFunctionArgType.Address => Base58.Bitcoin.Decode((string) obj).ToArray(),
                ContractFunctionArgType.String => Encoding.ASCII.GetBytes((string) obj),
                ContractFunctionArgType.Uint64 => BitConverter.GetBytes((ulong) obj).Reverse().ToArray(),
                _ => new byte[] { }
            };

            return buf.ToHex();
        }
    }
}