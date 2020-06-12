using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using MediatR;
using Zeus.Features.Metrics.Core;
using Zeus.Handlers.Alerting.Send;

namespace Zeus.Features.Metrics.Behaviors
{
    public class TrackAlertsSentMetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMetrics _metrics;

        public TrackAlertsSentMetricsBehavior(IMetrics metrics)
        {
            _metrics = metrics;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is SendAlertRequest sendRequest)
            {
                var channel = sendRequest.Subscription?.Channel ?? "unknown";
                var tags = new MetricTags("channel", channel);

                try
                {
                    var result = await next();

                    _metrics.Measure.Counter.Increment(MetricsRegistry.Counters.AlertsSent, tags);
                    _metrics.Measure.Counter.Increment(MetricsRegistry.Counters.AlertsSentTotal, tags);

                    return result;

                }
                catch
                {
                    _metrics.Measure.Counter.Increment(MetricsRegistry.Counters.AlertsSentFailed, tags);
                    _metrics.Measure.Counter.Increment(MetricsRegistry.Counters.AlertsSentFailedTotal, tags);

                    throw;
                }
            }

            return await next();
        }
    }
}
