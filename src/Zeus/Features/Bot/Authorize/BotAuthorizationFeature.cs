using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Zeus.v2.Handlers.Bot.Authorization;
using Zeus.v2.Shared.AppFeature;

namespace Zeus.v2.Features.Bot.Authorize
{
    public class BotAuthorizationFeature : AppFeature<BotAuthorizationFeatureOptions>
    {
        public BotAuthorizationFeature(
            IConfiguration configuration, 
            IHostEnvironment environment, 
            IOptions<BotAuthorizationFeatureOptions> options) : base(configuration, environment, options)
        {
        }

        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizeBotActionRequestBehavior<,>));
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
        }
    }
}