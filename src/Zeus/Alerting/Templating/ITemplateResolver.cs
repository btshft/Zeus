using System.Threading;
using System.Threading.Tasks;
using Zeus.Alerting.Routing;

namespace Zeus.Alerting.Templating
{
    public interface ITemplateResolver
    {
        Task<AlertTemplate> ResolveAsync(AlertRoute route, CancellationToken cancellation = default);
    }
}