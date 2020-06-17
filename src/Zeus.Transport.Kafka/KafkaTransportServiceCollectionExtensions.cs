using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zeus.Transport.Kafka.Internal;

namespace Zeus.Transport.Kafka
{
    public static class KafkaTransportServiceCollectionExtensions
    {
        public static IKafkaTransportBuilder AddKafkaTransport(this IServiceCollection services, Action<KafkaOptions> configure)
        {
            services.AddOptions<KafkaOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            services.AddSingleton<KafkaProducerProvider>();
            services.AddSingleton<KafkaTopicProvider>();

            services.AddHostedService<KafkaTopicCreationService>();

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<KafkaOptions>>();
                return options.Value.Producer;
            });

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<KafkaOptions>>();
                return options.Value.Consumer;
            });

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<KafkaOptions>>();
                return options.Value.Admin;
            });

            return new KafkaTransportBuilder(services);
        }
    }
}