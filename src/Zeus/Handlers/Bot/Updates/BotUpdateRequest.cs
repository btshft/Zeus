using MediatR;
using Telegram.Bot.Types;

namespace Zeus.v2.Handlers.Bot.Updates
{
    public class BotUpdateRequest : IRequest
    {
        public Update Update { get; }

        public BotUpdateRequest(Update update)
        {
            Update = update;
        }
    }
}