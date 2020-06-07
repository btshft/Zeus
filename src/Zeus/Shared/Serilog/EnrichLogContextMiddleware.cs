using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Serilog.Core;
using Zeus.Shared.Serilog.Enrichers;

namespace Zeus.Shared.Serilog
{
    public class EnrichLogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public EnrichLogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var enrichers = new ILogEventEnricher[]
            {
                new TraceIdEnricher(context.RequestServices.GetRequiredService<IHttpContextAccessor>()) 
            };

            using (LogContext.Push(enrichers))
            {
                await _next(context);
            }
        }
    }
}