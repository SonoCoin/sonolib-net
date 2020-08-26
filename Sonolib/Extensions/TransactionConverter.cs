using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using NBitcoin;
using Sonolib.Dtos;

namespace Sonolib.Extensions
{
    public class TransactionConverter
    {
        /// <summary>
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        // ReSharper disable once MemberCanBePrivate.Global
        public static T Parse<T>(string hex)
        {
            try
            {
                var bytes = hex.HexDecode();
                var str = Encoding.UTF8.GetString(bytes);
                return FromJson<T>(str);
            }
            catch (FormatException ex)
            {
                throw new FormatException(
                    $"String '{hex}' could not be converted to string (not hex?).", ex);
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        // ReSharper disable once MemberCanBePrivate.Global
        public static TransactionDto Parse(string hex, Network network)
        {
            try
            {
                var bytes = hex.HexDecode();
                var str = Encoding.UTF8.GetString(bytes);
                return FromJson(str);
            }
            catch (FormatException ex)
            {
                throw new FormatException(
                    $"String '{hex}' could not be converted to string (not hex?).", ex);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public TransactionDto Parse(string hex)
        {
            return Parse(hex, Network.Main);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prettify"></param>
        /// <returns></returns>
        private static T FromJson<T>(string str, bool prettify = true)
        {
            var opts = new JsonSerializerOptions
            {
                WriteIndented = prettify,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            };

            return JsonSerializer.Deserialize<T>(str, opts);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prettify"></param>
        /// <returns></returns>
        private static TransactionDto FromJson(string str, bool prettify = true)
        {
            var opts = new JsonSerializerOptions
            {
                WriteIndented = prettify,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            };

            return JsonSerializer.Deserialize<TransactionDto>(str, opts);
        }
    }
}