using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Zeus.Transport.InMemory
{
    public static class InMemoryTransportServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryTransport<TMessage>(this IServiceCollection services)
        {
            services.TryAddSingleton(sp => new InMemoryTransportChannel<TMessage>());
            services.AddHostedService(sp =>
            {
                var channel = sp.GetRequiredService<InMemoryTransportChannel<TMessage>>();
                return new InMemoryTransportService<TMessage>(
                    channel.Reader, sp, sp.GetRequiredService<ILoggerFactory>());
            });

            services.TryAddSingleton<ITransport<TMessage>>(sp =>
            {
                var channel = sp.GetRequiredService<InMemoryTransportChannel<TMessage>>();
                return new InMemoryTransport<TMessage>(channel.Writer);
            });

            return services;
        }

        public static IServiceCollection AddInMemoryTransportConsumer<TMessage, TConsumer>(this IServiceCollection services)
            where TConsumer : class, ITransportConsumer<TMessage>
        {
            services.TryAddSingleton(sp => new InMemoryTransportChannel<TMessage>());
            services.TryAddTransient<ITransportConsumer<TMessage>, TConsumer>();

            return services;
        }
    }
}