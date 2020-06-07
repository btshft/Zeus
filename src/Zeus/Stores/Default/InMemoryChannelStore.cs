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

        /// <inheritdoc />
        public Task<AlertsChannel[]> GetAsync(IEnumerable<string> names, CancellationToken cancellation = default)
        {
            if (names == null) 
                throw new ArgumentNullException(nameof(names));

            var channels = _channels.Where(c => names.Contains(c.Name))
                .Distinct()
                .ToArray();

            return Task.FromResult(channels);
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string name, CancellationToken cancellation = default)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            var exists = _channels.Any(c => c.Name == name);
            return Task.FromResult(exists);
        }
    }
}
