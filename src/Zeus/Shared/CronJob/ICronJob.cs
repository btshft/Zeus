using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Shared.CronJob
{
    public interface ICronJob
    {
        Task ExecuteAsync(CancellationToken cancellation = default);
    }
}