using System.Net.Http;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Zeus.v2.BackgroundServices;
using Zeus.v2.Features.Bot.Authorize;
using Zeus.v2.Handlers.Bot.Context;
using Zeus.v2.Handlers.Bot.Reply;
using Zeus.v2.Services.Telegram.Messaging;
using Zeus.v2.Shared.AppFeature;
using Zeus.v2.Shared.Extensions;
using Zeus.v2.Shared.Features.Extensions;
using Zeus.v2.Shared.Mediatr;

namespace Zeus.v2.Features.Bot
{
    public class BotFeature : AppFeature<BotFeatureOptions>
    {
        /// <inheritdoc />
        public BotFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<BotFeatureOptions> options)
            : base(configuration, environment, options)
        {
        }


        /// <inheritdoc />
        public override void Configure(IServiceCollection services, IAppFeatureCollection features)
        {
            const string clientName = "telegram.client";

            var options = Options.Value;

            services.AddHttpClient(clientName)
                .ConfigureHttpClient((sp, cl) => {})
                .ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    var proxy = options.Proxy?.CreateProxy();
                    return new HttpClientHandler
                    {
                        UseProxy = proxy != null,
                        Proxy = proxy
                    };
                });

            services.AddSingleton<IRequestHandlerFinder>(new RequestHandlerFinder(services));
            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                var client = sp.GetHttpClient(clientName);
                return new TelegramBotClient(options.Token, client);
            });

            services.AddHostedService<BotUpdatesPollingService>();

            services.AddSingleton<ITelegramMessageSender, TelegramMessageSender>();
            services.AddSingleton<IBotActionContextAccessor, BotActionContextAccessor>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(BotActionContextInitBehavior<,>));
            features.AddFromConfiguration<BotAuthorizationFeature, BotAuthorizationFeatureOptions>(
                "Bot:Authorization", required: true);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ReplyOnExceptionBehavior<,>));
        }

        /// <inheritdoc />
        public override void Use(IApplicationBuilder builder)
        {
        }
    }
}