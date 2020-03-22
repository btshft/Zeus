using Telegram.Bot.Types;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Requests
{
    public class ProcessUnrecognizedUpdate : IBotCommandRequest
    {
        public Update Update { get; }

        public ProcessUnrecognizedUpdate(Update update)
        {
            Update = update;
        }
    }
}