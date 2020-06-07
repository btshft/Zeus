using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Zeus.v2.Shared.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static TOptions GetOptions<TOptions>(this IServiceProvider provider) 
            where TOptions : class, new()
        {
            var optionsProvider = provider.GetRequiredService<IOptions<TOptions>>();
            return optionsProvider.Value;
        }

        public static ILogger<TContext> GetLogger<TContext>(this IServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();
            return loggerFactory?.CreateLogger<TContext>();
        }

        public static HttpClient GetHttpClient(this IServiceProvider provider, string name)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return clientFactory.CreateClient(name);
        }
    }
}