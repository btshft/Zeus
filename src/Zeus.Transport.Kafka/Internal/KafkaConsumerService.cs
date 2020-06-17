using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Zeus.Transport.Kafka.Internal
{
    internal class KafkaConsumerService<TMessage> : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumer<Null, TMessage> _consumer;
        private readonly ILogger<KafkaConsumerService<TMessage>> _logger;
        private readonly KafkaTopicProvider _topicProvider;

        public KafkaConsumerService(
            IServiceProvider serviceProvider,
            IOptions<KafkaOptions> optionsProvider,
            ILogger<KafkaConsumerService<TMessage>> logger,
            ConsumerConfig consumerConfig,
            KafkaTopicProvider topicProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _topicProvider = topicProvider;
            _consumer = new ConsumerBuilder<Null, TMessage>(consumerConfig)
                .SetLogHandler((consumer, message) =>
                {
                    var logLevel = message.Level.GetLogLevel();
                    logger.Log(logLevel, "Kafka log event {Name} ({Facility}) - {Message}", message.Name, message.Facility, message.Message);
                })
                .SetErrorHandler((consumer, error) =>
                {
                    var logLevel = error.GetLogLevel();
                    var category = error.GetCategory();

                    logger.Log(logLevel, "Kafka consumer error '{Code}' - '{Reason}', category '{Category}'", error.Code, error.Reason, category);
                })
                .SetValueDeserializer(new JsonDeserializer<TMessage>(optionsProvider.Value.CreateJsonOptions()))
                .Build();
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topic = _topicProvider.GetTopic<TMessage>();

            _consumer.Subscribe(topic);

            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var result = await Task.Run(() =>
                {
                    try
                    {
                        return _consumer.Consume(stoppingToken);
                    }
                    catch (ConsumeException ce)
                    {
                        _logger.LogError(ce,"Kafka consumer failed with exception");

                        return null;
                    }

                }, stoppingToken);

                if (result == null)
                    continue;

                var transportConsumers = _serviceProvider.GetServices<ITransportConsumer<TMessage>>();
                foreach (var transportConsumer in transportConsumers)
                {
                    try
                    {
                        _logger.LogInformation($"Consuming message of type '{typeof(TMessage).Name}'...");
                        await transportConsumer.ConsumeAsync(result.Message.Value, stoppingToken);
                        _logger.LogInformation($"Message of type '{typeof(TMessage).Name}' consumed.");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"Exception occured on consuming message of type '{typeof(TMessage).Name}'");
                    }
                }
            }
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer?.Close();
            return base.StopAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            _consumer?.Dispose();
        }
    }
}