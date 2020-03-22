using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Zeus.Bot.Notifications.Handlers
{
    public class BotExceptionHandler : INotificationHandler<BotExceptionOccured>
    {
        private readonly ILogger<BotExceptionHandler> _logger;

        public BotExceptionHandler(ILogger<BotExceptionHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(BotExceptionOccured notification, CancellationToken cancellationToken)
        {
            _logger.LogError(notification.Exception, $"[{notification.Exception.GetType().Name}]: {notification.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}