using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Zeus.Handlers.Bot.Consumers
{
    public class SendTelegramReply
    {
        public SendTelegramReply(ChatId chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }

        public ChatId ChatId { get; }

        public string Text { get; }

        public ParseMode ParseMode { get; set; } = ParseMode.Default;

        public bool DisableWebPagePreview { get; set; }

        public bool DisableNotification { get; set; }

        public int ReplyToMessageId { get; set; }
    }
}
