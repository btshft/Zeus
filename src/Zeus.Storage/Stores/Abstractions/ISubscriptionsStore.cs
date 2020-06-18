using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;

namespace Zeus.Storage.Stores.Abstractions
{
    public interface ISubscriptionsStore
    {
        Task<IReadOnlyCollection<AlertsSubscription>> GetAsync(string channel, CancellationToken cancellation = default);

        Task<IReadOnlyCollection<AlertsSubscription>> GetAsync(long chatId, CancellationToken cancellation = default);

        Task<AlertsSubscription> GetAsync(long chatId, string channel, CancellationToken cancellation = default);

        Task<IReadOnlyCollection<AlertsSubscription>> GetAllAsync(CancellationToken cancellation = default);

        Task<bool> ExistsAsync(string channel, long chatId, CancellationToken cancellation = default);

        Task StoreAsync(AlertsSubscription subscription, CancellationToken cancellation = default);

        Task RemoveAsync(AlertsSubscription subscription, CancellationToken cancellation = default);
    }
}