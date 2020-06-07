using MediatR;
using Telegram.Bot.Types;

namespace Zeus.v2.Handlers.Bot.Notifications
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
