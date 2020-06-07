using Telegram.Bot.Types;

namespace Zeus.v2.Handlers.Bot.Context
{
    public class BotActionContext
    {
        public Update Update { get; set; }

        public string TraceId { get; set; }

        public bool IsAuthorized { get; set; }

        public bool IsAnonymous { get; set; }
    }
}