using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Zeus.v2.Clients.Callback;
using Zeus.v2.Shared.Extensions;

namespace Zeus.v2.BackgroundServices
{
    public class BotUpdatesPollingService : BackgroundService
    {
        private readonly ILogger<BotUpdatesPollingService> _logger;
        private readonly ICallbackClient _callbackClient;

        private readonly Lazy<QueuedUpdateReceiver> _receiverProvider;

        public BotUpdatesPollingService(
            ITelegramBotClient client,
            ILogger<BotUpdatesPollingService> logger, 
            ICallbackClient callbackClient)
        {
            _logger = logger;
            _callbackClient = callbackClient;
            _receiverProvider = new Lazy<QueuedUpdateReceiver>(() => new QueuedUpdateReceiver(client));
        }

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Task HandleException(Exception exception, CancellationToken cancellation)
            {
                _logger.LogWarning(exception, "Exception occured while receiving updates");
                return Task.CompletedTask;
            }

            var receiver = _receiverProvider.Value;

            receiver.StartReceiving(errorHandler: HandleException, cancellationToken: cancellationToken);

            return base.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_receiverProvider.IsValueCreated)
                _receiverProvider.Value.StopReceiving();

            return base.StopAsync(cancellationToken);
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiver = _receiverProvider.Value;

            await foreach (var update in receiver.YieldUpdatesAsync().WithCancellation(stoppingToken))
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
