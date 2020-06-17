using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zeus.Features.Bot;
using Zeus.Features.Localization;
using Zeus.Features.Metrics;
using Zeus.Features.Metrics.Behaviors;
using Zeus.Features.Profiling;
using Zeus.Features.Profiling.Behaviors;
using Zeus.Handlers.Alerting.Webhook;
using Zeus.Handlers.Bot.Authorization;
using Zeus.Handlers.Bot.Context;
using Zeus.Handlers.Bot.Reply;
using Zeus.Shared.AppFeature;
using Zeus.Shared.AppFeature.Extensions;
using Zeus.Shared.Mediatr;

namespace Zeus.Features.Mediatr
{
    public class MediatrFeature : AppFeature
    {
        public MediatrFeature(IConfiguration configuration, IHostEnvironment environment) 
            : base(configuration, environment)
        {
        }

        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
        {
            services.AddMediatR(typeof(AlertManagerUpdateRequestHandler));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WrapExceptionsBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LocalizationBehavior<,>));

            if (features.IsEnabled<ProfilingFeature>())
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ProfilingBehavior<,>));
            }

            if (features.IsEnabled<BotFeature>())
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(BotActionContextInitBehavior<,>));
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizeBotActionRequestBehavior<,>));
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ReplyOnExceptionBehavior<,>));
            }

            if (features.IsEnabled<MetricsFeature>())
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TrackAlertsSentMetricsBehavior<,>));
            }
        }
    }
}