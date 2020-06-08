using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Zeus.Services.Telegram;

namespace Zeus.Shared.Extensions
{
    public static class TelegramClientExtensions
    {
        public static async Task<Message[]> SendTextMessageSplitAsync(this ITelegramBotClient client, ChatId chatId,
            string text,
            ParseMode parseMode = default,
            bool disableWebPagePreview = default,
            bool disableNotification = default,
            int replyToMessageId = default,
            IReplyMarkup replyMarkup = default,
            CancellationToken cancellationToken = default)
        {
            const int maxLength = 4096;
            if (text.Length <= maxLength)
            {
                var message = await client.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview, disableNotification,
                    replyToMessageId, replyMarkup, cancellationToken);

                return new[] { message };
            }

            var texts = text.SplitBy(maxLength - 6).ToArray();
            var results = new List<Message>();
            var delimiter = "...".Escape(parseMode);

            foreach (var (index, messagePart) in texts.Index())
            {
                var isFirst = index == 0;
                var isLast = index == texts.Length - 1;
                var message = isFirst
                    ? $"{messagePart}{delimiter}"
                    : isLast
                        ? $"{delimiter}{messagePart}"
                        : $"{delimiter}{messagePart}{delimiter}";

                var messageIdToReply = replyToMessageId != 0 && isFirst ? replyToMessageId : 0;
                var result = await client.SendTextMessageAsync(chatId, message, parseMode, disableWebPagePreview,
                    disableNotification, replyToMessageId: messageIdToReply, replyMarkup, cancellationToken);

                results.Add(result);
            }

            return results.ToArray();
        }
    }
}