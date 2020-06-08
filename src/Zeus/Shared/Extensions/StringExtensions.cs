using System;
using System.Collections.Generic;

namespace Zeus.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceFirst(this string text, string search, string replace, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (text == null) 
                throw new ArgumentNullException(nameof(text));

            if (search == null) 
                throw new ArgumentNullException(nameof(search));

            if (replace == null) 
                throw new ArgumentNullException(nameof(replace));

            var position = text.IndexOf(search, comparison);
            if (position < 0)
                return text;

            return text[..position] + replace + text[(position + search.Length)..];
        }

        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            if (str == null) 
                throw new ArgumentNullException(nameof(str));

            if (chunkLength < 1)
                throw new ArgumentException("Chunks length cannot should be >=1", nameof(chunkLength));

            if (str == string.Empty)
            {
                yield return str;
                yield break;
            }

            for (var i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }
    }
}