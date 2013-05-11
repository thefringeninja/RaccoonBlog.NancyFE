using System;
using System.Text.RegularExpressions;

namespace RaccoonBlog.NancyFE
{
    public static class InflectorExtensions
    {
        private static readonly Regex AlphaNumericWhiteSpace = new Regex(@"([^A-Za-z0-9\s])", RegexOptions.Compiled);

        public static string Slugify(this string word)
        {
            return AlphaNumericWhiteSpace.Replace(word, String.Empty)
                                         .Underscore().Dasherize();
        }
    }
}