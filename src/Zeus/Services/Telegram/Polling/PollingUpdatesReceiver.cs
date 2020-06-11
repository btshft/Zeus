using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Features.Bot;

namespace Zeus.Services.Telegram.Polling
{
    public class PollingUpdatesReceiver : IDisposable, IPollingUpdatesReceiver
    {
        private readonly ITelegramBotClient _client;
        private readonly object _lock;
        private readonly Channel<Update> _updatesChannel;
        private readonly ILogger<PollingUpdatesReceiver> _logger;
        private readonly IOptions<BotFeatureOptions> _botOptions;

        private bool _isReceiving;
        private CancellationTokenSource _pauseTokenSource;

        public PollingUpdatesReceiver(
            ITelegramBotClient client, 
            ILogger<PollingUpdatesReceiver> logger, 
            IOptions<BotFeatureOptions> botOptions)
        {
            _client = client;
            _logger = logger;
            _botOptions = botOptions;
            _lock = new object();
            _updatesChannel = Channel.CreateUnbounded<Update>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = true
            });
        }

        private async Task<Update[]> GetUpdatesAsync(IEnumerable<UpdateType> allowedUpdates, int offset, CancellationToken cancellation = default)
        {
            var timeout = (int)_client.Timeout.TotalSeconds;
            return await _client.MakeRequestAsync(new GetUpdatesRequest
            {
                Offset = offset,
                Timeout = timeout,
                AllowedUpdates = allowedUpdates,
            }, cancellation);
        }

        public void StartReceiving(
            UpdateType[] allowedUpdates = default,
            Func<Exception, CancellationToken, Task> errorHandler = default,
            CancellationToken cancellationToken = default)
        {
            // ReSharper disable once RedundantAssignment
            var pauseToken = default(CancellationToken);

            lock (_lock)
            {
                if (_isReceiving)
                    throw new InvalidOperationException("Already receiving updates");

                if (cancellationToken.IsCancellationRequested)
                    return;

                _isReceiving = true;

                if (_pauseTokenSource != null)
                {
                    _pauseTokenSource.Dispose();
                    _pauseTokenSource = null;
                }

                _pauseTokenSource = new CancellationTokenSource();

                // Stop updates receiving if user cancels start
                cancellationToken.Register(() => _pauseTokenSource?.Cancel());
                pauseToken = _pauseTokenSource.Token;
            }

            var breaker = Policy<Update[]>
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: _botOptions.Value.PollingAttemptsBeforeBreaking,
                    durationOfBreak: _botOptions.Value.PollingDurationOfBreak,
                    onBreak: (result, _1, _2) =>
                    {
                        _logger.LogInformation(result.Exception, "Telegram polling circuit broken");
                    }, 
                    onReset: _ =>
                    {
                        _logger.LogInformation("Telegram polling circuit resets");
                    });

            var fallback = Policy<Update[]>
                .Handle<Exception>()
                .FallbackAsync(Array.Empty<Update>());

            var policy = Policy.WrapAsync(fallback, breaker);

            // ReSharper disable once MethodSupportsCancellation
            // CT handled inside of task
            Task.Run(async () =>
            {
                var writer = _updatesChannel.Writer;
                var offset = 0;

                try
                {
                    while (!pauseToken.IsCancellationRequested)
                    {
                        var data = new Dictionary<string, object>
                        {
                            { nameof(offset), offset }
                        };

                        if (breaker.CircuitState == CircuitState.Open)
                        {
                            _logger.LogInformation($"Waiting '{_botOptions.Value.PollingDurationOfBreak}' because circuit is in open state");
                            await Task.Delay(_botOptions.Value.PollingDurationOfBreak, pauseToken);
                        }

                        var updates = await policy.ExecuteAsync(async (ctx) =>
                        {
                            try
                            {
                                var updatesOffset = (int)ctx[nameof(offset)];
                                return await GetUpdatesAsync(allowedUpdates, updatesOffset, pauseToken);
                            }
                            catch (OperationCanceledException)
                            {
                                // Ignore
                                return Array.Empty<Update>();
                            }
                            catch (Exception e)
                            {
                                if (errorHandler != null)
                                    await errorHandler(e, pauseToken);

                                throw;
                            }
                        }, contextData: data);

                        if (updates.Length > 0)
                        {
                            offset = updates[^1].Id + 1;

                            foreach (var update in updates)
                                await writer.WriteAsync(update, cancellationToken);
                        }
                    }
                }
                finally
                {
                    lock (_lock)
                    {
                        _pauseTokenSource?.Dispose();
                        _pauseTokenSource = null;
                        _isReceiving = false;
                    }
                }
            });
        }

        public void StopReceiving()
        {
            lock (_lock)
            { 
                if (!_isReceiving || _pauseTokenSource == null)
                    return;

                _pauseTokenSource.Cancel();
            }
        }

        public virtual async IAsyncEnumerable<Update> YieldUpdatesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (_updatesChannel == null)
                throw new InvalidOperationException("Receiver is not started");

            var reader = _updatesChannel.Reader;

            while (await reader.WaitToReadAsync(cancellationToken))
                while (reader.TryRead(out var update))
                    yield return update;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _pauseTokenSource?.Dispose();
            _updatesChannel.Writer.TryComplete();
        }
    }
}