using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Zeus.Services.Telegram.Polling
{
    public interface IPollingUpdatesReceiver
    {
        void StartReceiving(
            UpdateType[] allowedUpdates = default,
            Func<Exception, CancellationToken, Task> errorHandler = default,
            CancellationToken cancellationToken = default);

        void StopReceiving();

        IAsyncEnumerable<Update> YieldUpdatesAsync(CancellationToken cancellationToken = default);
    }
}