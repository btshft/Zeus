using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Bot.Notifications
{
    public class BotUpdateReceived : INotification
    {
        public Update Update { get; }

        public BotUpdateReceived(Update update)
        {
            Update = update;
        }
    }
}
