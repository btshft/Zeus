using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Shared.Diagnostics
{
    public static class DiagnosticObserverServiceCollectionExtensions
    {
        public static IServiceCollection AddDiagnosticObserver<TObserver>(
            this IServiceCollection services)
            where TObserver : DiagnosticObserver
        {
            var descriptorExists = services.Any(d => d.ServiceType == typeof(TObserver));
            if (descriptorExists)
                return services;

            services.AddTransient<TObserver>();
            services.AddTransient<DiagnosticObserver>(sp => sp.GetRequiredService<TObserver>());

            return services;
        }
    }
}