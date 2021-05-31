using System;
using System.Security.Cryptography;
using System.Text;

namespace WebLicense.Logic.Auxiliary.Extensions
{
    public static class StringExtensions
    {
        public static string GetRandom(this string @string, int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var res = new StringBuilder();
            using var rng = new RNGCryptoServiceProvider();
            var uintBuffer = new byte[sizeof(uint)];

            while (length-- > 0)
            {
                rng.GetBytes(uintBuffer);
                var num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(valid[(int) (num % (uint) valid.Length)]);
            }

            return res.ToString();        }
    }
}