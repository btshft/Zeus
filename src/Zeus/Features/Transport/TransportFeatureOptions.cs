using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace Zeus.Features.Transport
{
    public class TransportFeatureOptions
    {
        public KafkaTransportOptions Kafka { get; set; }

        public class KafkaTransportOptions
        {
            public AdminClientConfig Admin { get; set; }

            [Required]
            public ProducerConfig Producer { get; set; }

            [Required]
            public ConsumerConfig Consumer { get; set; }

            public TransportOptions Alerts { get; set; }

            public TransportOptions Messages { get; set; }

            public class TransportOptions
            {
                public bool Enabled { get; set; }

                public AutoCreateTopicOptions AutoCreateTopic { get; set; }
            }

            public class AutoCreateTopicOptions
            {
                public short ReplicationFactor { get; set; }

                public int Partitions { get; set; }
            }
        }
    }
}