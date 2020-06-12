using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Zeus.BackgroundServices;
using Zeus.Handlers.Bot.Context;
using Zeus.Services.Telegram.Polling;
using Zeus.Shared.AppFeature;
using Zeus.Shared.Extensions;
using Zeus.Shared.Mediatr;

namespace Zeus.Features.Bot
{
    public class BotFeature : AppFeature<BotFeatureOptions>
    {
        /// <inheritdoc />
        public BotFeature(IConfiguration configuration, IHostEnvironment environment, IOptions<BotFeatureOptions> options)
            : base(configuration, environment, options)
        {
        }


        /// <inheritdoc />
        protected override void ConfigureFeature(IServiceCollection services, IAppFeatureCollection features)
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

            services.AddTransient<IBotActionContextAccessor, BotActionContextAccessor>();
            services.AddTransient<IBotUserProvider, BotUserProvider>();
            services.AddSingleton<IBotPollingUpdatesReceiver, BotPollingUpdatesReceiver>();
        }
    }
}