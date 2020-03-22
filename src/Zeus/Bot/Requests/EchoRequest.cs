using Telegram.Bot.Types;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Requests
{
    public class EchoRequest : IBotCommandRequest
    {
        public EchoRequest(Update update)
        {
            Update = update;
        }

        public Update Update { get; }
    }
}