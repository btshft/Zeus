using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Confluent.Kafka;

namespace Zeus.Transport.Kafka
{
    public class KafkaOptions
    {
        public AdminClientConfig Admin { get; set; }

        [Required]
        public ProducerConfig Producer { get; set; }

        [Required]
        public ConsumerConfig Consumer { get; set; }

        public Action<JsonSerializerOptions> ConfigureJson { get; set; }

        public Func<Type, string> TopicNameFormatter { get; set; } = type => type.FullName
            .Replace(".", "-")
            .ToLowerInvariant();

        public TopicDefinition[] AutoCreateTopics { get; set; } = Array.Empty<TopicDefinition>();

        public class TopicDefinition
        {
            public Type MessageType { get; set; }

            public int Partitions { get; set; }

            public short ReplicationFactor { get; set; }
        }

        internal JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            if (ConfigureJson != null)
                ConfigureJson(options);

            return options;
        }
    }
}