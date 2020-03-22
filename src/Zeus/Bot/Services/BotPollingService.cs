using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Zeus.Bot.Notifications;

namespace Zeus.Bot.Services
{
    public class BotPollingService : IHostedService
    {
        private readonly IMediator _mediator;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly Lazy<QueuedUpdateReceiver> _receiverProvider;

        public BotPollingService(ITelegramBotClient client, IHostApplicationLifetime lifetime, IMediator mediator)
        {
            _lifetime = lifetime;
            _mediator = mediator;
            _receiverProvider = new Lazy<QueuedUpdateReceiver>(() => new QueuedUpdateReceiver(client));
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            async Task HandleException(Exception exception, CancellationToken cancellation)
            {
                await _mediator.Publish(new BotExceptionOccured(exception), cancellation);
            }

            var receiver = _receiverProvider.Value;

            receiver.StartReceiving(errorHandler: HandleException, cancellationToken: _lifetime.ApplicationStopping);

            Task.Run(async () =>
            {
                await foreach (var update in receiver.YieldUpdatesAsync().WithCancellation(_lifetime.ApplicationStopping))
                {
                    await _mediator.Publish(new BotUpdateReceived(update), _lifetime.ApplicationStopping);
                }
            }, _lifetime.ApplicationStopping);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_receiverProvider.IsValueCreated)
                _receiverProvider.Value.StopReceiving();

            return Task.CompletedTask;
        }
    }
}
