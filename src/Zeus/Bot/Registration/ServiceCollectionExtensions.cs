using System;
using System.Linq;
using System.Net;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MihaZupan;
using Telegram.Bot;
using Zeus.Bot.Authorization;
using Zeus.Bot.Options;
using Zeus.Bot.Requests;
using Zeus.Bot.Requests.Abstractions;
using Zeus.Bot.Services;
using Zeus.Bot.State;
using Zeus.Extensions;

namespace Zeus.Bot.Registration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBot(this IServiceCollection services, Action<BotOptions> configure)
        {
            static HttpToSocks5Proxy Socks5Proxy(BotOptions.Socks5ProxyOptions options)
            {
                var proxy = !string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password)
                    ? new HttpToSocks5Proxy(options.Address, options.Port, options.Username, options.Password)
                    : new HttpToSocks5Proxy(options.Address, options.Port);

                proxy.ResolveHostnamesLocally = true;

                return proxy;
            }

            static WebProxy HttpProxy(BotOptions.HttpProxyOptions options)
            {
                var httpProxy = new WebProxy(options.Address, options.Port);
                if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
                {
                    httpProxy.Credentials = new NetworkCredential(options.Username, options.Password);
                }

                return httpProxy;
            }

            services.AddOptions<BotOptions>()
                .Configure(configure)
                .ValidateDataAnnotations();

            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                var options = sp.GetOptions<BotOptions>();
                if (options.Proxy?.Socks5 != null)
                    return new TelegramBotClient(options.Token, Socks5Proxy(options.Proxy.Socks5));

                return options.Proxy?.Http != null 
                    ? new TelegramBotClient(options.Token, HttpProxy(options.Proxy.Http)) 
                    : new TelegramBotClient(options.Token);
            });

            services.AddHostedService<BotPollingService>();
            services.AddTransient<IBotStateStorage, BotStateStorage>();

            foreach (var type in typeof(SubscribeOnNotifications).Assembly
                .GetExportedTypes()
                .Where(t => t.IsConcrete() && t.IsAssignableTo<IBotCommandRequest>()))
            {
                services.TryAddTransient(
                    typeof(IPipelineBehavior<,>).MakeGenericType(type, typeof(Unit)), 
                    typeof(UsernameBasedAuthorizationBehavior<>).MakeGenericType(type));
            }

            return services;
        }
    }
}
