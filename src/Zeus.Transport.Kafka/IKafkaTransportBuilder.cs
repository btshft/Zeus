using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Transport.Kafka
{
    public interface IKafkaTransportBuilder
    {
        IServiceCollection Services { get; }
    }
}