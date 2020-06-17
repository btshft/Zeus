using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Zeus.Handlers.Bot.Updates;
using Zeus.Services.Telegram.Polling;
using Zeus.Shared.Extensions;

namespace Zeus.BackgroundServices
{
    public class BotUpdatesPollingService : BackgroundService
    {
        private readonly ILogger<BotUpdatesPollingService> _logger;
        private readonly IMediator _mediator;
        private readonly IBotPollingUpdatesReceiver _receiver;

        public BotUpdatesPollingService(
            ILogger<BotUpdatesPollingService> logger,
            IMediator mediator, 
            IBotPollingUpdatesReceiver receiver)
        {
            _logger = logger;
            _mediator = mediator;
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
                    await _mediator.Send(new BotUpdateRequest(update), cancellationToken: stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception occured while processing bot update:{Environment.NewLine}{update.ToJson()}");
                }
            }
        }
    }
}
