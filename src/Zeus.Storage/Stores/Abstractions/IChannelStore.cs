using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;

namespace Zeus.Storage.Stores.Abstractions
{
    public interface IChannelStore
    {
        Task<AlertsChannel> GetAsync(string name, CancellationToken cancellation = default);
        Task<AlertsChannel[]> GetAsync(IEnumerable<string> names, CancellationToken cancellation = default);
        Task<bool> ExistsAsync(string name, CancellationToken cancellation = default);
    }
}