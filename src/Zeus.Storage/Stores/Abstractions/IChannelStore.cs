using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;

namespace Zeus.Storage.Stores.Abstractions
{
    public interface IChannelStore
    {
        Task<AlertsChannel> GetAsync(string name, CancellationToken cancellation = default);
        Task<IReadOnlyCollection<AlertsChannel>> GetAllAsync(CancellationToken cancellation = default);
    }
}