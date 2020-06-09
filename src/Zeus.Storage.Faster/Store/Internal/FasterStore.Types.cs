using System;
using System.Text.Json;
using FASTER.core;
using Microsoft.Extensions.Logging;

namespace Zeus.Storage.Faster.Store.Internal
{
    public class FasterStoreException : Exception
    {
        internal FasterStoreException(string message)
            : base(message)
        {
        }

        internal FasterStoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    internal partial class FasterStore<TKey, TValue>
    {
        public struct ValueHolder
        {
            public TValue Value;

            public ValueHolder(TValue value)
            {
                Value = value;
            }
        }

        public struct KeyHolder
        {
            public TKey Key;

            public KeyHolder(TKey key)
            {
                Key = key;
            }
        }

        public class StoreContext
        {
            public static StoreContext Null = new StoreContext();
        }

        public class StoreFunctions : IFunctions<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext>
        {
            private readonly ILogger<FasterStore<TKey, TValue>> _logger;

            public StoreFunctions(ILogger<FasterStore<TKey, TValue>> logger)
            {
                _logger = logger;
            }

            /// <inheritdoc />
            public void ReadCompletionCallback(ref KeyHolder key, ref ValueHolder input, ref ValueHolder output, StoreContext ctx,
                Status status)
            {
                if (!_logger.IsEnabled(LogLevel.Trace))
                    return;

                try
                {
                    var keyJson = key.Key != null ? JsonSerializer.Serialize(key.Key) : "null";
                    var outputJson = output.Value != null ? JsonSerializer.Serialize(output.Value) : "null";

                    _logger.LogTrace($"Read completed, key: '{keyJson}', output: '{outputJson}'");
                } catch 
                { }
            }

            /// <inheritdoc />
            public void UpsertCompletionCallback(ref KeyHolder key, ref ValueHolder value, StoreContext ctx)
            {
                if (!_logger.IsEnabled(LogLevel.Trace)) 
                    return;

                try
                {
                    var keyJson = key.Key != null ? JsonSerializer.Serialize(key.Key) : "null";
                    var valueJson = value.Value != null ? JsonSerializer.Serialize(value.Value) : "null";

                    _logger.LogTrace($"Upsert completed, key: '{keyJson}', input: '{valueJson}'");
                }
                catch
                { }
            }

            /// <inheritdoc />
            public void RMWCompletionCallback(ref KeyHolder key, ref ValueHolder input, StoreContext ctx, Status status)
            {
            }

            /// <inheritdoc />
            public void DeleteCompletionCallback(ref KeyHolder key, StoreContext ctx)
            {
                if (!_logger.IsEnabled(LogLevel.Trace))
                    return;

                try
                {
                    var keyJson = key.Key != null ? JsonSerializer.Serialize(key.Key) : "null";
                    _logger.LogTrace($"Delete completed, key: '{keyJson}'");
                }
                catch
                { }
            }

            /// <inheritdoc />
            public void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
            {
                _logger.LogTrace($"Checkpoint created, session-id '{sessionId}'");
            }

            /// <inheritdoc />
            public void InitialUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value)
            {
                value = input;
            }

            /// <inheritdoc />
            public void CopyUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder oldValue, ref ValueHolder newValue)
            {
                newValue = oldValue;
            }

            /// <inheritdoc />
            public bool InPlaceUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value)
            {
                value = input;
                return true;
            }

            /// <inheritdoc />
            public void SingleReader(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value, ref ValueHolder dst)
            {
                dst = value;
            }

            /// <inheritdoc />
            public void ConcurrentReader(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value, ref ValueHolder dst)
            {
                dst = value;
            }

            /// <inheritdoc />
            public void SingleWriter(ref KeyHolder key, ref ValueHolder src, ref ValueHolder dst)
            {
                dst = src;
            }

            /// <inheritdoc />
            public bool ConcurrentWriter(ref KeyHolder key, ref ValueHolder src, ref ValueHolder dst)
            {
                dst = src;
                return true;
            }
        }
    }
}