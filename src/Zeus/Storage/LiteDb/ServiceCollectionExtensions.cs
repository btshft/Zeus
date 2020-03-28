using System;
using Microsoft.Extensions.DependencyInjection;
using Zeus.Storage.Abstraction;

namespace Zeus.Storage.LiteDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLiteDb(this IServiceCollection services, Action<LiteDbOptions> configure)
        {
            services.AddOptions<LiteDbOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            services.AddSingleton<LiteDbProvider>();
            services.AddTransient(typeof(IStorage<>), typeof(LiteDbStorage<>));

            return services;
        }
    }
}