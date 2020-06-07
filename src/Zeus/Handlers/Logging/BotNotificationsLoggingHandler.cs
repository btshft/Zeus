using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Zeus.Handlers.Bot.Notifications;
using Zeus.Shared.Extensions;

namespace Zeus.Handlers.Logging
{
    public class BotNotificationsLoggingHandler : 
        INotificationHandler<BotUnknownCommandReceived>, 
        INotificationHandler<BotUnsupportedUpdateReceived>
    {
        private readonly ILogger<BotNotificationsLoggingHandler> _logger;

        public BotNotificationsLoggingHandler(ILogger<BotNotificationsLoggingHandler> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public Task Handle(BotUnknownCommandReceived notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Unknown or incomplete command received. Update: {Environment.NewLine}{notification.Update.ToJson()}");
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Handle(BotUnsupportedUpdateReceived notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Unsupported update received. Update: {Environment.NewLine}{notification.Update.ToJson()}");
            return Task.CompletedTask;
        }
    }
}
