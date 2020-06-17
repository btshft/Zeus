using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.Features.Cleanup.Jobs;
using Zeus.Shared.AppFeature;
using Zeus.Shared.CronJob;

namespace Zeus.Features.Cleanup
{
    public class CleanupFeature : AppFeature<CleanupFeatureOptions>
    {
        public CleanupFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<CleanupFeatureOptions> options) 
            : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            if (Options.Value.OrphanNotifications != null)
            {
                services.AddCronJob<OrphanSubscriptionsCleanupJob>(o =>
                {
                    o.UseLocalTimeZone = false;
                    o.Schedule = Options.Value.OrphanNotifications.Schedule;
                });
            }
        }
    }
}