using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Features.Metrics.Collectors
{
    public interface IMetricsCollector
    {
        Task CollectAsync(CancellationToken cancellation = default);
    }
}