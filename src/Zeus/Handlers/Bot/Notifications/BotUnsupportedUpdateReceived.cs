using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Notifications
{
    public class BotUnsupportedUpdateReceived : INotification
    {
        public BotUnsupportedUpdateReceived(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }
}
