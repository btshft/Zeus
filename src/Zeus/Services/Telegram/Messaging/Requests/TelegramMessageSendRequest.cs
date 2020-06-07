using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Zeus.Services.Telegram.Messaging.Requests
{
    public class TelegramMessageSendRequest
    {
        public ChatId ChatId { get; }

        public string Text { get; }

        public int ReplyToMessageId { get; set; }

        public ParseMode ParseMode { get; set; } = ParseMode.Default;

        public bool DisableNotification { get; set; }

        public TelegramMessageSendRequest(ChatId chatId, string text)
        {
            ChatId = chatId ?? throw new ArgumentNullException(nameof(chatId));
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}