using MediatR;
using Telegram.Bot.Types;

namespace Zeus.v2.Handlers.Bot.Notifications
{
    public class BotUnknownCommandReceived : INotification
    {
        public BotUnknownCommandReceived(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }
}