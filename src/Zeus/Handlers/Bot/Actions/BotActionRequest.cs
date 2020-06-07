using Telegram.Bot.Types;
using Zeus.v2.Handlers.Bot.Abstractions;

namespace Zeus.v2.Handlers.Bot.Actions
{
    public class BotActionRequest<TAction> : IBotActionRequest
        where TAction : IBotAction
    {
        public BotActionRequest(Update update, TAction action)
        {
            Update = update;
            Action = action;
            Message = update.Message;
            Chat = update.Message.Chat;
        }

        /// <inheritdoc />
        public Update Update { get; }

        /// <inheritdoc />
        public Message Message { get; }

        /// <inheritdoc />
        public Chat Chat { get; }

        /// <summary>
        /// Bot action (action).
        /// </summary>
        public TAction Action { get; }
    }
}