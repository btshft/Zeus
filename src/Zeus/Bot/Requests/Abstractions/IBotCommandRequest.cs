using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Bot.Requests.Abstractions
{
    public interface IBotCommandRequest : IRequest
    {
        Update Update { get; }
    }
}
