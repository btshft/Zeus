using System;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;

namespace Zeus.Services.Telegram
{
    public static class TelegramUtils
    {
        private static readonly Regex MarkdownEscapeRegEx = new Regex(@"(?<link>\[[^\][]*]\(http[^()]*\))|(?<characters>[-.+!?^$[\](){}\\])",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromSeconds(30));

        public static string Escape(string text, ParseMode parseMode)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return parseMode switch
            {
                ParseMode.Html => text,
                ParseMode.MarkdownV2 => EscapeMarkdown(text),
                _ => text
            };
        }

        private static string EscapeMarkdown(string text)
        {
            static string Replace(Match match)
            {
                return string.IsNullOrWhiteSpace(match.Groups["link"].Value)
                    ? $"\\{match.Value}"
                    : match.Value;
            }

            return string.IsNullOrWhiteSpace(text) 
                ? text : MarkdownEscapeRegEx.Replace(text, Replace);
        }
    }
}