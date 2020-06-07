using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Zeus.v2.Shared.Serilog.Enrichers
{
    public class DemistifyExceptionEnricher : ILogEventEnricher
    {
        /// <inheritdoc />
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception != null)
                logEvent.Exception.Demystify();
        }
    }
}