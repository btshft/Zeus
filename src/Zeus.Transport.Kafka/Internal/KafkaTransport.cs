using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Zeus.Transport.Kafka.Internal
{
    internal class KafkaTransport<TMessage> : ITransport<TMessage>
    {
        private readonly KafkaProducerProvider _producerProvider;
        private readonly KafkaTopicProvider _topicProvider;

        public KafkaTransport(KafkaProducerProvider producerProvider, KafkaTopicProvider topicProvider)
        {
            _producerProvider = producerProvider;
            _topicProvider = topicProvider;
        }

        /// <inheritdoc />
        public async Task SendAsync(TMessage value, CancellationToken cancellation = default)
        {
            var producer = _producerProvider.GetProducer<TMessage>();
            var topic = _topicProvider.GetTopic<TMessage>();

            var message = new Message<Null, TMessage>
            {
                Value = value,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            await producer.ProduceAsync(topic, message, cancellation);
        }
    }
}
