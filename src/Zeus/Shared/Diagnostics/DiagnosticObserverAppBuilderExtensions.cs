using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Shared.Diagnostics
{
    public static class DiagnosticObserverAppBuilderExtensions
    {
        public static IApplicationBuilder UseDiagnosticObserver<TObserver>(this IApplicationBuilder builder)
            where TObserver : DiagnosticObserver
        {
            var observer = builder.ApplicationServices.GetService<TObserver>();
            if (observer != null)
            {
                DiagnosticListener.AllListeners.Subscribe(observer);
            }

            return builder;
        }
    }
}