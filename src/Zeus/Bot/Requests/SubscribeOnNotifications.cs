using Telegram.Bot.Types;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Requests
{
    public class SubscribeOnNotifications : IBotCommandRequest
    {
        public Update Update { get; }

        public SubscribeOnNotifications(Update update)
        {
            Update = update;
        }
    }
}
