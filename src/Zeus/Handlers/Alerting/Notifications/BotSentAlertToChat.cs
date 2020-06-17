using System;
using MediatR;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Alerting.Notifications
{
    public class BotSentAlertToChat : INotification
    {
        public BotSentAlertToChat(Message[] messages)
        {
            Messages = messages ?? Array.Empty<Message>();
        }

        public Message[] Messages { get; }
    }
}
