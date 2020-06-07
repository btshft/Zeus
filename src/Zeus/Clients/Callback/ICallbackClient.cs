﻿using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Zeus.v2.Clients.Callback
{
    public interface ICallbackClient
    {
        Task HandleBotUpdateAsync(Update update, CancellationToken cancellation = default);
    }
}
