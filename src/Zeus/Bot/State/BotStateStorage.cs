using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Telegram.Bot.Types;
using Zeus.Storage.Abstraction;
using Zeus.Storage.Extensions;

namespace Zeus.Bot.State
{
    public class BotStateStorage : IBotStateStorage
    {
        private static readonly AsyncReaderWriterLock Lock = new AsyncReaderWriterLock();

        private readonly IStorage<BotState> _innerStorage;

        public BotStateStorage(IStorage<BotState> innerStorage)
        {
            _innerStorage = innerStorage;
        }

        /// <inheritdoc />
        public async Task<IBotState> GetStateAsync(CancellationToken cancellation = default)
        {
            using (await Lock.ReaderLockAsync(cancellation))
            {
                var existedState = await _innerStorage.FirstOrDefaultAsync(cancellation);
                if (existedState != null) 
                    return existedState;
            }

            using (await Lock.WriterLockAsync(cancellation))
            {
                var state = new BotState
                {
                    Conversations = new List<Chat>()
                };

                await _innerStorage.PersistAsync(state, cancellation);

                return state;
            }
        }

        /// <inheritdoc />
        public async Task<bool> ClearStateAsync(CancellationToken cancellation = default)
        {
            using (await Lock.WriterLockAsync(cancellation))
            {
                return await _innerStorage.DeleteAsync(s => true, cancellation);
            }
        }

        /// <inheritdoc />
        public async Task ModifyStateAsync(Func<IBotState, CancellationToken, Task> modify,
            CancellationToken cancellation = default)
        {
            using (await Lock.WriterLockAsync(cancellation))
            {
                var state = await _innerStorage.FirstOrDefaultAsync(cancellation) ??
                            new BotState
                            {
                                Conversations = new List<Chat>()
                            };

                await modify(state, cancellation);
                await _innerStorage.PersistAsync(state, cancellation);
            }
        }
    }
}