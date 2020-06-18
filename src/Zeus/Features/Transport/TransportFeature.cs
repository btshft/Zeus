using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Handlers.Alerting.Consumers;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Shared.AppFeature;
using Zeus.Transport;
using Zeus.Transport.InMemory;
using Zeus.Transport.Kafka;

namespace Zeus.Features.Transport
{
    public class TransportFeature : AppFeature<TransportFeatureOptions>
    {
        /// <inheritdoc />
        public TransportFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<TransportFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            void RegisterKafkaTransport<TMessage, TConsumer>(IKafkaTransportBuilder builder, 
                TransportFeatureOptions.KafkaTransportOptions.TransportOptions transportOptions) 
                    where TConsumer : class, ITransportConsumer<TMessage>
            {
                if (transportOptions != null && transportOptions.Enabled)
                {
                    builder.AddTransport<TMessage>()
                        .AddConsumer<TMessage, TConsumer>();
                }
                // In memory for alerts
                else
                {
                    services
                        .AddInMemoryTransport<TMessage>()
                        .AddInMemoryTransportConsumer<TMessage, TConsumer>();
                }
            }

            KafkaOptions.TopicDefinition ConvertDefinition<TMessage>(
                TransportFeatureOptions.KafkaTransportOptions.TransportOptions transportOptions)
            {
                if (transportOptions == null || !transportOptions.Enabled || transportOptions.AutoCreateTopic == null)
                    return null;

                return new KafkaOptions.TopicDefinition
                {
                    ReplicationFactor = transportOptions.AutoCreateTopic.ReplicationFactor,
                    Partitions = transportOptions.AutoCreateTopic.Partitions,
                    MessageType = typeof(TMessage)
                };
            }

            var options = Options.Value;

            if (options.Kafka == null)
            {
                services.AddInMemoryTransport<SendTelegramReply>()
                    .AddInMemoryTransportConsumer<SendTelegramReply, SendTelegramReplyConsumer>();

                services
                    .AddInMemoryTransport<SendTelegramAlert>()
                    .AddInMemoryTransportConsumer<SendTelegramAlert, SendTelegramAlertConsumer>();
            }
            else
            {
                var builder = services.AddKafkaTransport(o =>
                {
                    o.Admin = options.Kafka.Admin;
                    o.Consumer = options.Kafka.Consumer;
                    o.Producer = options.Kafka.Producer;

                    var alertsTopicDefinition = ConvertDefinition<SendTelegramAlert>(options.Kafka.Alerts);
                    var messagesTopicDefinition = ConvertDefinition<SendTelegramReply>(options.Kafka.Messages);

                    o.AutoCreateTopics = new[] { alertsTopicDefinition, messagesTopicDefinition}
                        .Where(d => d != null)
                        .ToArray();
                });

                RegisterKafkaTransport<SendTelegramReply, SendTelegramReplyConsumer>(builder, options.Kafka.Messages);
                RegisterKafkaTransport<SendTelegramAlert, SendTelegramAlertConsumer>(builder, options.Kafka.Alerts);
            }
        }
    }
}