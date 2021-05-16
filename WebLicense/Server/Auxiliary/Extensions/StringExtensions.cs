using System;
using System.Linq;

namespace WebLicense.Server.Auxiliary.Extensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string @string, int length)
        {
            if (length < 1) throw new ArgumentOutOfRangeException(nameof(length));

            var count = @string.Length / length;
            var chunks = Enumerable.Range(0, count).Select(q => @string.Substring(q * length, length)).ToArray();
            return chunks;
        }
    }
}