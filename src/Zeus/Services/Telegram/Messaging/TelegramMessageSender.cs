using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Services.Telegram.Messaging.Requests;

namespace Zeus.Services.Telegram.Messaging
{
    public class TelegramMessageSender : ITelegramMessageSender
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MessageSanitizer _sanitizer;

        private static readonly Regex MarkdownSanitizeRegEx = new Regex(@"(?<link>\[[^\][]*]\(http[^()]*\))|(?<characters>[-.+?^$[\](){}\\])",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public TelegramMessageSender(ITelegramBotClient botClient)
        {
            _botClient = botClient;
            _sanitizer = new MessageSanitizer();
        }

        /// <inheritdoc />
        public Task<Message> SendAsync(TelegramMessageSendRequest request, CancellationToken cancellation = default)
        {
            var text = _sanitizer.Sanitize(request.Text, request.ParseMode);

            return _botClient.SendTextMessageAsync(request.ChatId, text, request.ParseMode, disableWebPagePreview: true,
                disableNotification: request.DisableNotification, request.ReplyToMessageId,
                cancellationToken: cancellation);
        }

        public class MessageSanitizer
        {
            public string Sanitize(string text, ParseMode parseMode)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return text;

                return parseMode switch
                {
                    ParseMode.Html => text,
                    ParseMode.MarkdownV2 => SanitizeMarkdown(text),
                    _ => text
                };
            }

            private static string SanitizeMarkdown(string text)
            {
                static string Replace(Match match)
                {
                    return string.IsNullOrWhiteSpace(match.Groups["link"].Value)
                        ? $"\\{match.Value}"
                        : match.Value;
                }

                if (string.IsNullOrWhiteSpace(text))
                    return text;

                return MarkdownSanitizeRegEx.Replace(text, Replace);
            }
        }
    }
}