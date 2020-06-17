using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Transport.Kafka.Internal
{
    internal class KafkaTransportBuilder : IKafkaTransportBuilder
    {
        public KafkaTransportBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}