using NBitcoin;

namespace Sonolib.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Generate seed from mnemonic phrase
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static byte[] ToSeed(this string words)
        {
            var mnemonic = new Mnemonic(words);
            return mnemonic.DeriveSeed();
        }
        
        /// <summary>
        /// Normalize network name
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToNetwork(this string str)
        {
            return str.ToLower().Remove("net").Capitalize();
        }
        
        /// <summary>
        /// Normalize netowrk name, add 'Net' to the end.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToNetworkNormalized(this string str)
        {
            return $"{str.ToLower().Remove("net").Capitalize()}Net";
        }
    }
}