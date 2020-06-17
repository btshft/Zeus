using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Transport
{
    public interface ITransportConsumer<in T>
    {
        Task ConsumeAsync(T value, CancellationToken cancellation = default);
    }
}