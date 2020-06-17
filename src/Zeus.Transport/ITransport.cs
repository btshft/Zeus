using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Transport
{
    public interface ITransport<in T>
    {
        Task SendAsync(T value, CancellationToken cancellation = default);
    }
}
