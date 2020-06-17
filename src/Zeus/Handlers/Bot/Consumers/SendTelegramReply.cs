using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Zeus.Handlers.Bot.Consumers
{
    public class SendTelegramReply
    {
        public SendTelegramReply(long chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }

        // ReSharper disable once UnusedMember.Global
        public SendTelegramReply()
        { }

        public long ChatId { get; set; }

        public string Text { get; set; }

        public ParseMode ParseMode { get; set; } = ParseMode.Default;

        public bool DisableWebPagePreview { get; set; }

        public bool DisableNotification { get; set; }

        public int ReplyToMessageId { get; set; }
    }
}
