using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Notifications
{
    public class BotReceivedUnsupportedUpdate : INotification
    {
        public BotReceivedUnsupportedUpdate(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }
}
