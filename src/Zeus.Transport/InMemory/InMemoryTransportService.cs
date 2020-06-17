using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zeus.Transport.InMemory
{
    internal class InMemoryTransportService<T> : BackgroundService
    {
        private readonly ChannelReader<T> _reader;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InMemoryTransportService<T>> _logger;

        public InMemoryTransportService(
            ChannelReader<T> reader, 
            IServiceProvider serviceProvider, 
            ILoggerFactory loggerFactory)
        {
            _reader = reader;
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<InMemoryTransportService<T>>();
        }


        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _reader.WaitToReadAsync(stoppingToken))
            {
                while (_reader.TryRead(out var item))
                {
                    var consumers = _serviceProvider.GetServices<ITransportConsumer<T>>();

                    foreach (var consumer in consumers)
                    {
                        try
                        {
                            _logger.LogInformation($"Consuming message of type '{typeof(T).Name}'...");
                            await consumer.ConsumeAsync(item, stoppingToken);
                            _logger.LogInformation($"Message of type '{typeof(T).Name}' consumed.");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Exception occured on consuming message of type '{typeof(T).Name}'");
                        }
                    }
                }
            }
        }
    }
}