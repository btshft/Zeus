using Telegram.Bot.Types;
using Zeus.Bot.Requests.Abstractions;

namespace Zeus.Bot.Requests
{
    /// <summary>
    /// Request to get info about current user.
    /// </summary>
    public class CurrentUserInfoRequest : IBotCommandRequest
    {
        public Update Update { get; }

        public CurrentUserInfoRequest(Update update)
        {
            Update = update;
        }
    }
}