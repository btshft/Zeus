using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Context
{
    public class BotActionContext
    {
        public Update Update { get; set; }

        public User Bot { get; set; }

        public string TraceId { get; set; }

        public bool IsAuthorized { get; set; }

        public bool IsAnonymous { get; set; }
    }
}