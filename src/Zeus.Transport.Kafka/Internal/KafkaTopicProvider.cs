using Microsoft.Extensions.Options;

namespace Zeus.Transport.Kafka.Internal
{
    internal class KafkaTopicProvider
    {
        private readonly IOptions<KafkaOptions> _optionsProvider;

        public KafkaTopicProvider(IOptions<KafkaOptions> optionsProvider)
        {
            _optionsProvider = optionsProvider;
        }

        public string GetTopic<TMessage>()
        {
            var options = _optionsProvider.Value;
            return options.TopicNameFormatter(typeof(TMessage));
        }
    }
}