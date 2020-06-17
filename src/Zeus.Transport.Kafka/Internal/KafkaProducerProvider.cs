using System;
using System.Collections.Concurrent;
using System.Linq;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Zeus.Transport.Kafka.Internal
{
    internal class KafkaProducerProvider : IDisposable
    {
        private readonly ConcurrentDictionary<Type, object> _producers;
        private readonly IOptions<KafkaOptions> _optionsProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ProducerConfig _producerConfig;

        public KafkaProducerProvider(IOptions<KafkaOptions> optionsProvider, ILoggerFactory loggerFactory, ProducerConfig producerConfig)
        {
            _optionsProvider = optionsProvider;
            _loggerFactory = loggerFactory;
            _producerConfig = producerConfig;
            _producers = new ConcurrentDictionary<Type, object>();
        }

        public IProducer<Null, TMessage> GetProducer<TMessage>()
        {
            var options = _optionsProvider.Value;
            var producer = _producers.GetOrAdd(typeof(TMessage), type =>
            {
                var logger = _loggerFactory.CreateLogger<IProducer<Null, TMessage>>();

                var builder = new ProducerBuilder<Null, TMessage>(_producerConfig)
                    .SetValueSerializer(new JsonSerializer<TMessage>(options.CreateJsonOptions()))
                    .SetErrorHandler((p, e) =>
                    {
                        var logLevel = e.GetLogLevel();
                        var category = e.GetCategory();

                        logger.Log(logLevel, "Kafka producer error '{Code}' - '{Reason}', category '{Category}'", e.Code, e.Reason, category);
                    })
                    .SetLogHandler((p, m) =>
                    {
                        var logLevel = m.Level.GetLogLevel();
                        logger.Log(logLevel, "Kafka log event {Name} ({Facility}) - {Message}", m.Name, m.Facility, m.Message);
                    });

                return builder.Build();
            });

            return (IProducer<Null, TMessage>) producer;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var producer in _producers.Select(p => p.Value).Cast<IDisposable>())
                producer.Dispose();
        }
    }
}