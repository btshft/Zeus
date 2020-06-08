using Microsoft.Extensions.Localization;
using Telegram.Bot.Types.Enums;

namespace Zeus.Services.Telegram
{
    public static class TelegramStringExtensions
    {
        public static string Escape(this string str, ParseMode parseMode)
        {
            return string.IsNullOrWhiteSpace(str)
                ? str
                : TelegramUtils.Escape(str, parseMode);
        }

        public static string EscapeMarkdown(this string str)
        {
            return str.Escape(ParseMode.MarkdownV2);
        }

        public static LocalizedString Escape(this LocalizedString str, ParseMode parseMode)
        {
            return new LocalizedString(str.Name, str.Value.Escape(parseMode), str.ResourceNotFound, str.SearchedLocation);
        }

        public static LocalizedString EscapeMarkdown(this LocalizedString str)
        {
            return str.Escape(ParseMode.MarkdownV2);
        }
    }
}