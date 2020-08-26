using System;
using System.Linq;

namespace Sonolib.Extensions
{
    // @TODO need check
    public static class ByteArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        
        public static bool StartWith(this byte[] data, byte[] versionBytes)
        {
            if (data.Length < versionBytes.Length)
                return false;
            return !versionBytes
                .Where((t, index) => (int) data[index] != (int) t)
                .Any();
        }

        public static byte[] SafeSubarray(this byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof (array));
            if (offset < 0 || offset > array.Length)
                throw new ArgumentOutOfRangeException(nameof (offset));
            if (count < 0 || offset + count > array.Length)
                throw new ArgumentOutOfRangeException(nameof (count));
            if (offset == 0 && array.Length == count)
                return array;
            byte[] numArray = new byte[count];
            Buffer.BlockCopy(array, offset, numArray, 0, count);
            return numArray;
        }

        // ReSharper disable once UnusedMember.Global
        public static byte[] SafeSubarray(this byte[] array, int offset)
        {
            if (array == null)
                throw new ArgumentNullException(nameof (array));
            if (offset < 0 || offset > array.Length)
                throw new ArgumentOutOfRangeException(nameof (offset));
            var count = array.Length - offset;
            var numArray = new byte[count];
            Buffer.BlockCopy(array, offset, numArray, 0, count);
            return numArray;
        }

        public static byte[] Concat(this byte[] arr, params byte[][] arrs)
        {
            var numArray = new byte[arr.Length + arrs.Sum(a => a.Length)];
            Buffer.BlockCopy(arr, 0, numArray, 0, arr.Length);
            var length = arr.Length;
            foreach (var arr1 in arrs)
            {
                Buffer.BlockCopy(arr1, 0, numArray, length, arr1.Length);
                length += arr1.Length;
            }
            return numArray;
        }
    }
}