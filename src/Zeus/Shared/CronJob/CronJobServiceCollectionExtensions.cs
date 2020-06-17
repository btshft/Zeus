using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Shared.CronJob
{
    public static class CronJobServiceCollectionExtensions
    {
        public static IServiceCollection AddCronJob<TJob>(this IServiceCollection services,
            Action<CronJobOptions> configureOptions) where TJob : class, ICronJob
        {
            services.AddOptions<CronJobOptions>(typeof(TJob).FullName)
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            services.AddScoped<TJob>();
            services.AddHostedService<CronJobService<TJob>>();

            return services;
        }
    }
}