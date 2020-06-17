using Microsoft.Extensions.DependencyInjection;
using Zeus.Transport.Kafka.Internal;

namespace Zeus.Transport.Kafka
{
    public static class KafkaTransportBuilderExtensions
    {
        public static IKafkaTransportBuilder AddTransport<TMessage>(this IKafkaTransportBuilder builder)
        {
            builder.Services.AddTransient<ITransport<TMessage>, KafkaTransport<TMessage>>();
            return builder;
        }

        public static IKafkaTransportBuilder AddConsumer<TMessage, TConsumer>(this IKafkaTransportBuilder builder)
            where TConsumer : class, ITransportConsumer<TMessage>
        {
            builder.Services.AddHostedService<KafkaConsumerService<TMessage>>();
            builder.Services.AddTransient<ITransportConsumer<TMessage>, TConsumer>();

            return builder;
        }
    }
}