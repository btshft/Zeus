using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Zeus.Shared.Utils
{
    public static class UrlHelper
    {
        public static string Combine(string baseUrl, params string[] parts)
        {
            static void AddPart(ref StringBuilder builder, string relativePart)
            {
                builder = builder
                    .Append('/')
                    .Append(relativePart.TrimStart('/'));
            }

            if (string.IsNullOrWhiteSpace(baseUrl)) 
                throw new ArgumentNullException(nameof(baseUrl));

            var builder = new StringBuilder(baseUrl.TrimEnd('/'));
            while (parts.Length != 0)
            {
                AddPart(ref builder, parts[0]);
                parts = parts[1..];
            }

            return builder.ToString();
        }
    }
}
