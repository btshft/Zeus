using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Shared.Transport
{
    public interface ITransport<in T>
    {
        Task SendAsync(T value, CancellationToken cancellation = default);
    }
}
