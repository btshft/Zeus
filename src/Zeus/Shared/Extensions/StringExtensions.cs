using System;

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
    }
}