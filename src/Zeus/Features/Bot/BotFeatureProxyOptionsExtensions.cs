using System;
using System.Net;
using MihaZupan;

namespace Zeus.v2.Features.Bot
{
    public static class BotFeatureProxyOptionsExtensions
    {
        public static IWebProxy CreateProxy(this BotFeatureOptions.ProxyOptions options)
        {
            IWebProxy Socks5Proxy()
            {
                var proxy = !string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password)
                    ? new HttpToSocks5Proxy(options.Address, options.Port, options.Username, options.Password)
                    : new HttpToSocks5Proxy(options.Address, options.Port);

                proxy.ResolveHostnamesLocally = true;

                return proxy;
            }

            IWebProxy HttpProxy()
            {
                var httpProxy = new WebProxy(options.Address, options.Port);
                if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
                {
                    httpProxy.Credentials = new NetworkCredential(options.Username, options.Password);
                }

                return httpProxy;
            }

            return options.Type switch
            {
                BotFeatureOptions.ProxyType.Http => HttpProxy(),
                BotFeatureOptions.ProxyType.Socks5 => Socks5Proxy(),
                _ => throw new NotSupportedException($"Proxy of type '{options.Type}' not supported")
            };
        }
    }
}