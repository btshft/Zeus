using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Notifications
{
    public class BotRepliedToChat : INotification
    {
        public BotRepliedToChat(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}