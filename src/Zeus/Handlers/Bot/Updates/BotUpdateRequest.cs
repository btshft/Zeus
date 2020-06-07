using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Updates
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