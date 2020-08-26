using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sonolib.Extensions
{
    /// <summary/>
    public static class HexByteConvertorExtensions
    {
        private static readonly byte[] Empty = new byte[0];

        /// <summary/>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToHex(this IEnumerable<byte> value, bool prefix = false)
        {
            return (prefix ? "0x" : "") + 
                   string.Concat(value
                           .Select(b => b.ToString("x2"))
                           .ToArray());
        }

        /// <summary/>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool HasHexPrefix(this string value)
        {
            return value.StartsWith("0x");
        }

        /// <summary/>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveHexPrefix(this string value)
        {
            return value.Substring(value.StartsWith("0x") ? 2 : 0);
        }

        /// <summary/>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsTheSameHex(this string first, string second)
        {
            return string.Equals(
                first.EnsureHexPrefix(), 
                second.EnsureHexPrefix(),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary/>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string EnsureHexPrefix(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return !value.HasHexPrefix() ? $"0x{value}" : value;
        }

        /// <summary/>
        /// <param name="values"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static string[] EnsureHexPrefix(this string[] values)
        {
            if (values == null)
            {
                return values;
            }

            foreach (var str in values)
            {
                str.EnsureHexPrefix();
            }

            return values;
        }

        /// <summary/>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHexCompact(this IEnumerable<byte> value)
        {
            return value.ToHex().TrimStart('0');
        }

        /// <summary/>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HexToByteArrayInternal(string value)
        {
            byte[] numArray;
            if (string.IsNullOrEmpty(value))
            {
                numArray = Empty;
            }
            else
            {
                var length = value.Length;
                var index1 = value.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0;
                var num1 = index1;
                var num2 = length - num1;
                var flag = false;
                if (num2 % 2 != 0)
                {
                    flag = true;
                    ++num2;
                }

                numArray = new byte[num2 / 2];
                var num3 = 0;
                if (flag)
                {
                    numArray[num3++] = FromCharacterToByte(value[index1], index1);
                    ++index1;
                }

                for (var index2 = index1; index2 < value.Length; index2 += 2)
                {
                    var num4 = FromCharacterToByte(value[index2], index2, 4);
                    var num5 = FromCharacterToByte(value[index2 + 1], index2 + 1);
                    numArray[num3++] = (byte) (num4 | (uint) num5);
                }
            }

            return numArray;
        }

        /// <summary/>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static byte[] HexToByteArray(this string value)
        {
            try
            {
                return HexToByteArrayInternal(value);
            }
            catch (FormatException ex)
            {
                throw new FormatException(
                    $"String '{value}' could not be converted to byte array (not hex?).", ex);
            }
        }

        /// <summary/>
        /// <param name="character"></param>
        /// <param name="index"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            var num = (byte) character;
            if (64 < num && 71 > num || 96 < num && 103 > num)
            {
                if (64 == (64 & num))
                    num = 32 != (32 & (int) num)
                        ? (byte) (num + 10 - 65 << shift)
                        : (byte) (num + 10 - 97 << shift);
            }
            else
            {
                if (41 >= num || 64 <= num)
                    throw new FormatException(
                        $"Character '{character.ToString(CultureInfo.InvariantCulture)}' " + 
                        $"at index '{index.ToString(CultureInfo.InvariantCulture)}' is not valid alphanumeric character.");
                num = (byte) (num - 48 << shift);
            }

            return num;
        }
        
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        
        public static byte[] HexDecode(this string str)
        {
            return HexByteConvertorExtensions.HexToByteArrayInternal(str);
        }

    }
}
