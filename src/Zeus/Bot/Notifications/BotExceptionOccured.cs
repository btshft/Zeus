using System;
using MediatR;

namespace Zeus.Bot.Notifications
{
    public class BotExceptionOccured : INotification
    {
        public Exception Exception { get; }

        public BotExceptionOccured(Exception exception)
        {
            Exception = exception;
        }
    }
}