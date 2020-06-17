using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Zeus.Handlers.Alerting.Consumers
{
    public class SendTelegramAlert 
    {
        public SendTelegramAlert(ChatId chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }

        public ChatId ChatId { get; }

        public string Text { get; }

        public ParseMode ParseMode { get; set; } = ParseMode.Default;

        public bool DisableNotification { get; set; }
    }
}