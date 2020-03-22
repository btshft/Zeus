using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Bot.State
{
    public static class BotStateServiceExtensions
    {
        public static Task ModifyStateAsync(this IBotStateStorage storage, Action<IBotState> modify,
            CancellationToken cancellation = default)
        {
            return storage.ModifyStateAsync((m, ct) =>
            {
                modify(m);
                return Task.CompletedTask;
            }, cancellation);
        }
    }
}