using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Notifications
{
    public class BotReceivedUnknownCommand : INotification
    {
        public BotReceivedUnknownCommand(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }
}