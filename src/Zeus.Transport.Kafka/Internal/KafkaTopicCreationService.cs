using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Transport.Kafka.Internal
{
    internal class KafkaTopicCreationService : IHostedService
    {
        private readonly IOptions<KafkaOptions> _optionsProvider;
        private readonly ILogger<KafkaTopicCreationService> _logger;
        private readonly AdminClientConfig _adminClientConfig;

        public KafkaTopicCreationService(IOptions<KafkaOptions> optionsProvider, ILogger<KafkaTopicCreationService> logger, AdminClientConfig adminClientConfig)
        {
            _optionsProvider = optionsProvider;
            _logger = logger;
            _adminClientConfig = adminClientConfig;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var options = _optionsProvider.Value;
            if (options.AutoCreateTopics.Length < 1)
                return;

            var adminClient = new AdminClientBuilder(_adminClientConfig)
                .SetLogHandler((client, message) =>
                {
                    var logLevel = message.Level.GetLogLevel();
                    _logger.Log(logLevel, "Kafka log event {Name} ({Facility}) - {Message}", message.Name,
                        message.Facility, message.Message);
                })
                .SetErrorHandler((client, error) =>
                {
                    var logLevel = error.GetLogLevel();
                    var category = error.GetCategory();

                    _logger.Log(logLevel, "Kafka consumer error '{Code}' - '{Reason}', category '{Category}'",
                        error.Code, error.Reason, category);
                })
                .Build();

            var specifications = options.AutoCreateTopics.Select(t => new TopicSpecification
            {
                Name = options.TopicNameFormatter(t.MessageType),
                NumPartitions = t.Partitions,
                ReplicationFactor = t.ReplicationFactor
            });

            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(15));
                var existingTopics = metadata.Topics;

                foreach (var specification in specifications)
                {
                    if (existingTopics.Any(t => t.Topic == specification.Name))
                        continue;

                    await adminClient.CreateTopicsAsync(new[] { specification });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create topics");
            }

            adminClient.Dispose();
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}