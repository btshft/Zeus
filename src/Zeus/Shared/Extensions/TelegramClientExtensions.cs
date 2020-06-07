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
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default(CancellationToken))
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

            foreach (var (index, messagePart) in texts.Index())
            {
                var isFirst = index == 0;
                var isLast = index == texts.Length - 1;
                var message = isFirst
                    ? $"{messagePart}..."
                    : isLast
                        ? $"...{messagePart}"
                        : $"...{messagePart}...";

                var messageIdToReply = replyToMessageId != 0 && isFirst ? replyToMessageId : 0;
                var result = await client.SendTextMessageAsync(chatId, message.Escape(parseMode), parseMode, disableWebPagePreview,
                    disableNotification, replyToMessageId: messageIdToReply, replyMarkup, cancellationToken);

                results.Add(result);
            }

            return results.ToArray();
        }
    }
}