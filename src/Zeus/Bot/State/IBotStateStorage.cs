using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Bot.State
{
    public interface IBotStateStorage
    {
        Task<IBotState> GetStateAsync(CancellationToken cancellation = default);

        Task<bool> ClearStateAsync(CancellationToken cancellation = default);

        Task ModifyStateAsync(Func<IBotState, CancellationToken, Task> modify, CancellationToken cancellation = default);
    }
}
