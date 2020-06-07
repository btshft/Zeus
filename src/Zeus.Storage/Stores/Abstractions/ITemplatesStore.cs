using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;

namespace Zeus.Storage.Stores.Abstractions
{
    public interface ITemplatesStore
    {
        Task<AlertsTemplate> GetAsync(string channel, CancellationToken cancellation = default);
    }
}
