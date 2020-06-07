using MediatR;
using Telegram.Bot.Types;

namespace Zeus.v2.Handlers.Bot.Abstractions
{
    public interface IBotActionRequest : IRequest
    {
        /// <summary>
        /// Bot update.
        /// </summary>
        public Update Update { get; }

        /// <summary>
        /// User's message.
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Chat.
        /// </summary>
        public Chat Chat { get; }
    }
}