using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Zeus.Transport.InMemory
{
    internal class InMemoryTransport<T> : ITransport<T>
    {
        private readonly ChannelWriter<T> _writer;

        public InMemoryTransport(ChannelWriter<T> writer)
        {
            _writer = writer;
        }

        /// <inheritdoc />
        public async Task SendAsync(T value, CancellationToken cancellation = default)
        {
            await _writer.WriteAsync(value, cancellation);
        }
    }
}