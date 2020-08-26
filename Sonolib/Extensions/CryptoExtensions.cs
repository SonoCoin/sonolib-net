﻿using System;
using System.Security.Cryptography;

namespace Sonolib.Extensions
{
    public static class CryptoExtension
    {
        /// <summary/>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] ToSha256(this byte[] val)
        {
            return SHA256.Create().ComputeHash(val);
        }

        /// <summary/>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] ToDoubleSha256(this byte[] val)
        {
            return val.ToSha256().ToSha256();
        }

        public static byte[] ToInt2(this int val)
        {
            return BitConverter.GetBytes(val);
        }
    }
}