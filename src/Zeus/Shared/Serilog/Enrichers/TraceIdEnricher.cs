using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Zeus.Shared.Serilog.Enrichers
{
    public class TraceIdEnricher : ILogEventEnricher
    {
        private const string PropertyName = "TraceId";

        private readonly IHttpContextAccessor _contextAccessor;

        public TraceIdEnricher(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <inheritdoc />
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = _contextAccessor.HttpContext;
            if (context != null)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty(PropertyName, new ScalarValue(context.TraceIdentifier)));
            }
        }
    }
}