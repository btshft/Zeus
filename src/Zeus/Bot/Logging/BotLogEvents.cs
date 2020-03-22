using Microsoft.Extensions.Logging;

namespace Zeus.Bot.Logging
{
    public static class BotLogEvents
    {
        public static EventId UserActionForbidden = new EventId(100, "User action forbidden");
    }
}