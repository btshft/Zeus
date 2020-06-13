using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Stores.Default
{
    public class InMemoryChannelStore : IChannelStore
    {
        private readonly HashSet<AlertsChannel> _channels;

        public InMemoryChannelStore(IEnumerable<AlertsChannel> channels)
        {
            _channels = new HashSet<AlertsChannel>(channels, new AlertsChannel.NameEqualityComparer());
        }

        /// <inheritdoc />
        public Task<AlertsChannel> GetAsync(string name, CancellationToken cancellation = default)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            var channel = _channels.FirstOrDefault(s => s.Name == name);
            return Task.FromResult(channel);
        }

        public Task<IReadOnlyCollection<AlertsChannel>> GetAllAsync(CancellationToken cancellation = default)
        {
            return Task.FromResult<IReadOnlyCollection<AlertsChannel>>(_channels.ToArray());
        }
    }
}
