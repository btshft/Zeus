using MediatR;
using Telegram.Bot.Types;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Requests
{
    /// <summary>
    /// Request to get info about current chat.
    /// </summary>
    public class CurrentChatInfoRequest : IBotCommandRequest
    {
        /// <inheritdoc />
        public Update Update { get; }

        public CurrentChatInfoRequest(Update update)
        {
            Update = update;
        }
    }
}
