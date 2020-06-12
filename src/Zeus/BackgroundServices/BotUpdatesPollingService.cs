using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Zeus.Clients.Callback;
using Zeus.Services.Telegram.Polling;
using Zeus.Shared.Extensions;

namespace Zeus.BackgroundServices
{
    public class BotUpdatesPollingService : BackgroundService
    {
        private readonly ILogger<BotUpdatesPollingService> _logger;
        private readonly ICallbackClient _callbackClient;
        private readonly IBotPollingUpdatesReceiver _receiver;

        public BotUpdatesPollingService(
            ILogger<BotUpdatesPollingService> logger, 
            ICallbackClient callbackClient, 
            IBotPollingUpdatesReceiver receiver)
        {
            _logger = logger;
            _callbackClient = callbackClient;
            _receiver = receiver;
        }

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Task HandleException(Exception exception, CancellationToken cancellation)
            {
                _logger.LogWarning(exception, "Exception occured while receiving updates");
                return Task.CompletedTask;
            }

            _receiver.StartReceiving(
                allowedUpdates: new[] { UpdateType.Message }, 
                errorHandler: HandleException, 
                cancellationToken: cancellationToken);

            return base.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _receiver.StopReceiving();
            return base.StopAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var update in _receiver.YieldUpdatesAsync(stoppingToken))
            {
                try
                {
                    await _callbackClient.HandleBotUpdateAsync(update, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception occured while performing callback request. Update:{Environment.NewLine}{update.ToJson()}");
                }
            }
        }
    }
}
