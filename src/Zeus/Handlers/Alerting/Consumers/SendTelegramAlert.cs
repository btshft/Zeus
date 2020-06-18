using Telegram.Bot.Types.Enums;

namespace Zeus.Handlers.Alerting.Consumers
{
    public class SendTelegramAlert 
    {
        public SendTelegramAlert(long chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }

        // ReSharper disable once UnusedMember.Global
        public SendTelegramAlert()
        { }

        public long ChatId { get; set; }

        public string Text { get; set; }

        public ParseMode ParseMode { get; set; } = ParseMode.Default;

        public bool DisableNotification { get; set; }
    }
}