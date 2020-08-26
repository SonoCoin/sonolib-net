using System.Linq;

namespace Sonolib.Helpers
{
    // @TODO need check
    public abstract class DataEncoder
    {
        // ReSharper disable once UnusedMember.Global
        public static bool IsSpace(char c)
        {
            switch (c)
            {
                case '\t':
                case '\n':
                case '\v':
                case '\f':
                case '\r':
                case ' ':
                    return true;
                default:
                    return false;
            }
        }

        internal DataEncoder()
        {
        }

        // ReSharper disable once UnusedMember.Global
        public string EncodeData(byte[] data)
        {
            return EncodeData(data, 0, data.Length);
        }

        public abstract string EncodeData(byte[] data, int offset, int count);

        // ReSharper disable once UnusedMemberInSuper.Global
        public abstract byte[] DecodeData(string encoded);
    }

    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming
    public class ASCIIEncoder : DataEncoder
    {
        public override byte[] DecodeData(string encoded)
        {
            return string.IsNullOrEmpty(encoded) 
                ? new byte[0] 
                : encoded.ToCharArray()
                    .Select(o => (byte) o)
                    .ToArray();
        }

        public override string EncodeData(byte[] data, int offset, int count)
        {
            return new string((data)
                .Skip(offset)
                .Take(count)
                .Select(o => (char) o)
                .ToArray()).Replace("\0", "");
        }
    }
}