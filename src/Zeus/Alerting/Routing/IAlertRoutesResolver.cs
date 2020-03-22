using System.Threading;
using System.Threading.Tasks;
using Zeus.Alerting.Models;

namespace Zeus.Alerting.Routing
{
    public interface IAlertRoutesResolver
    {
        Task<AlertRoutes> ResolveAsync(AlertManagerUpdate update, CancellationToken cancellation = default);
    }
}
