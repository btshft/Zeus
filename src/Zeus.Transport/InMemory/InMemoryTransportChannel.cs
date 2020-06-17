using System.Threading.Channels;

namespace Zeus.Transport.InMemory
{
    internal class InMemoryTransportChannel<T>
    {
        public InMemoryTransportChannel()
        {
            var channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true
            });

            Reader = channel.Reader;
            Writer = channel.Writer;
        }

        public ChannelReader<T> Reader { get; }

        public ChannelWriter<T> Writer { get; }
    }
}