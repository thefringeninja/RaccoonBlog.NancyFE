using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RaccoonBlog.NancyFE
{
    public static class HexExtensions
    {
        public static string ToUrlSafeBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
        }

        public static string ToHexString(this IEnumerable<byte> bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")).ToArray());
        }

        public static byte[] ParseHex(this string text)
        {
            if ((text.Length%2) != 0)
            {
                throw new ArgumentException("Invalid length: " + text.Length);
            }

            if (text.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Substring(2);
            }

            var arrayLength = text.Length/2;
            var byteArray = new byte[arrayLength];
            for (var i = 0; i < arrayLength; i++)
            {
                var substring = text.Substring(i*2, 2);
                byteArray[i] = byte.Parse(substring, NumberStyles.HexNumber);
            }

            return byteArray;
        }
    }
}