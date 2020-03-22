using Telegram.Bot.Types;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Requests
{
    public class UnsubscribeFromNotifications : IBotCommandRequest
    {
        public UnsubscribeFromNotifications(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }
}