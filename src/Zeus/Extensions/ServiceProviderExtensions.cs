using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Zeus.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static TOptions GetOptions<TOptions>(this IServiceProvider provider)
            where TOptions : class, new()
        {
            return provider.GetRequiredService<IOptions<TOptions>>().Value;
        }
    }
}