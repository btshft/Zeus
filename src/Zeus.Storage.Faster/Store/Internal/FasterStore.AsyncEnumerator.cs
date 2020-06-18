using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FASTER.core;
using Zeus.Storage.Faster.Utils;

namespace Zeus.Storage.Faster.Store.Internal
{
    internal partial class FasterStore<TKey, TValue>
    {
        private IterationState _iterationState;

        internal class IterationState : IDisposable
        {
            public IteratorStoreFunctions Functions;

            public long UntilAddress;

            public FasterKV<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, IteratorStoreFunctions> Store;

            public ClientSession<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, IteratorStoreFunctions> Session;

            public IFasterScanIterator<KeyHolder, ValueHolder> KeyIterator;

            public void Dispose()
            {
                Session?.TryDispose(out _);
                Store?.TryDispose(out _);
            }
        }


        internal class FasterAsyncEnumerator : IAsyncEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly FasterStore<TKey, TValue> _store;
            private readonly ValueTask _initEnumerator;

            public FasterAsyncEnumerator(FasterStore<TKey, TValue> store)
            {
                _store = store;
                _initEnumerator = InitAsync();
            }

            /// <inheritdoc />
            public KeyValuePair<TKey, TValue> Current { get; set; }

            /// <inheritdoc />
            public ValueTask DisposeAsync()
            {
                _store._iterationState.TryDispose(out _);
                _store._iterationState = null;
                return new ValueTask(Task.CompletedTask);
            }

            /// <inheritdoc />
            public async ValueTask<bool> MoveNextAsync()
            {
                try
                {
                    await _initEnumerator;

                    while (true)
                    {
                        var iterationState = _store._iterationState;
                        if (!iterationState.KeyIterator.GetNext(out var record, out var key, out var result))
                        {
                            return false;
                        }

                        if (record.Tombstone)
                            continue;

                        Current = new KeyValuePair<TKey, TValue>(key.Key, result.Value);
                        return true;
                    }

                }
                catch (Exception e)
                {
                    throw new FasterStoreException("Unable to move next", e);
                }
            }

            private async ValueTask InitAsync()
            {
                if (_store._iterationState != null)
                    throw new FasterStoreException("Store already in iteration state");

                try
                {
                    var keyValueStore = _store._keyValueStore;
                    var iteratorStore = new FasterKV<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, IteratorStoreFunctions>(keyValueStore.IndexSize,
                        new IteratorStoreFunctions(), new LogSettings(), serializerSettings: _store._serializerSettings, comparer: keyValueStore.Comparer);

                    var iteratorSession = iteratorStore.NewSession();
                    var iterationState = _store._iterationState = new IterationState
                    {
                        Store = iteratorStore,
                        Session = iteratorSession,
                        UntilAddress = _store._keyValueStore.Log.TailAddress,
                        Functions = new IteratorStoreFunctions()
                    };

                    using var storeIterator = keyValueStore.Log.Scan(keyValueStore.Log.BeginAddress, keyValueStore.Log.TailAddress);
                    while (storeIterator.GetNext(out var record, out var key, out var value))
                    {
                        if (record.Tombstone)
                            await iteratorSession.DeleteAsync(ref key);
                        else
                            await iteratorSession.UpsertAsync(ref key, ref value);
                    }

                    var scanUntil = iterationState.UntilAddress;

                    RemoveTombstones(
                        ref _store._iterationState.UntilAddress,
                        ref scanUntil,
                        ref _store._iterationState.Session);

                    _store._iterationState.KeyIterator = _store._iterationState.Store.Log.Scan(
                        _store._iterationState.Store.Log.BeginAddress,
                        _store._iterationState.Store.Log.TailAddress);
                }
                catch (Exception e)
                {
                    throw new FasterStoreException("Unable to create iterator", e);
                }
            }

            private void RemoveTombstones(ref long untilAddress, ref long scanUntil,
                ref ClientSession<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, IteratorStoreFunctions> session)
            {
                var store = _store._keyValueStore;
                while (scanUntil < store.Log.SafeReadOnlyAddress)
                {
                    untilAddress = scanUntil;
                    scanUntil = store.Log.SafeReadOnlyAddress;

                    using var iterator = store.Log.Scan(untilAddress, scanUntil);
                    while (iterator.GetNext(out _))
                    {
                        ref var key = ref iterator.GetKey();
                        session.Delete(ref key, userContext: default, serialNo: default);
                    }
                }
            }
        }

        internal class IteratorStoreFunctions : IFunctions<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext>
        {
            public void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public void ConcurrentReader(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public bool ConcurrentWriter(ref KeyHolder key, ref ValueHolder src, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                dst = src; return true;
            }

            public void CopyUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder oldValue, ref ValueHolder newValue) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public void InitialUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public bool InPlaceUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value) // lgtm[cs/too-many-ref-parameters]
            {
                return true;
            }

            public void ReadCompletionCallback(ref KeyHolder key, ref ValueHolder input, ref ValueHolder output, StoreContext ctx, Status status) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public void RMWCompletionCallback(ref KeyHolder key, ref ValueHolder input, StoreContext ctx, Status status) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public void SingleReader(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public void SingleWriter(ref KeyHolder key, ref ValueHolder src, ref ValueHolder dst) { dst = src; } // lgtm[cs/too-many-ref-parameters]

            public void UpsertCompletionCallback(ref KeyHolder key, ref ValueHolder value, StoreContext ctx) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            public void DeleteCompletionCallback(ref KeyHolder key, StoreContext ctx) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }
        }
    }
}