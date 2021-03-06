﻿using System.Text.Json;
using FASTER.core;
using Microsoft.Extensions.Logging;

namespace Zeus.Storage.Faster.Store.Internal
{
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
            public static readonly StoreContext Null = new StoreContext();
        }

        // ReSharper disable RedundantAssignment
        public class StoreFunctions : IFunctions<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext>
        {
            private readonly ILogger<FasterStore<TKey, TValue>> _logger;

            public StoreFunctions(ILogger<FasterStore<TKey, TValue>> logger)
            {
                _logger = logger;
            }

            /// <inheritdoc />
            public void ReadCompletionCallback(ref KeyHolder key, ref ValueHolder input, ref ValueHolder output, StoreContext ctx,
                Status status) // lgtm[cs/too-many-ref-parameters]
            {
                if (!_logger.IsEnabled(LogLevel.Trace))
                    return;

                string outputJson = "unknown", keyJson = "unknown";

                try
                {
                    keyJson = key.Key != null ? JsonSerializer.Serialize(key.Key) : keyJson;
                    outputJson = output.Value != null ? JsonSerializer.Serialize(output.Value) : outputJson;
                }
                finally
                {
                    _logger.LogTrace($"Read completed, key: '{keyJson}', output: '{outputJson}'");
                }
            }

            /// <inheritdoc />
            public void UpsertCompletionCallback(ref KeyHolder key, ref ValueHolder value, StoreContext ctx) // lgtm[cs/too-many-ref-parameters]
            {
                if (!_logger.IsEnabled(LogLevel.Trace)) 
                    return;

                string valueJson = "unknown", keyJson = "unknown";

                try
                {
                    keyJson = key.Key != null ? JsonSerializer.Serialize(key.Key) : keyJson;
                    valueJson = value.Value != null ? JsonSerializer.Serialize(value.Value) : valueJson;
                }
                finally
                {
                    _logger.LogTrace($"Upsert completed, key: '{keyJson}', input: '{valueJson}'");
                }
            }

            /// <inheritdoc />
            public void RMWCompletionCallback(ref KeyHolder key, ref ValueHolder input, StoreContext ctx, Status status) // lgtm[cs/too-many-ref-parameters]
            {
                // Method intentionally left empty.
            }

            /// <inheritdoc />
            public void DeleteCompletionCallback(ref KeyHolder key, StoreContext ctx)
            {
                if (!_logger.IsEnabled(LogLevel.Trace))
                    return;

                var keyJson = "unknown";

                try
                {
                    keyJson = key.Key != null ? JsonSerializer.Serialize(key.Key) : keyJson;
                }
                finally
                {
                    _logger.LogTrace($"Delete completed, key: '{keyJson}'");
                }
            }

            /// <inheritdoc />
            public void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint)
            {
                _logger.LogTrace($"Checkpoint created, session-id '{sessionId}'");
            }

            /// <inheritdoc />
            public void InitialUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value) // lgtm[cs/too-many-ref-parameters]
            {
                value = input;
            }

            /// <inheritdoc />
            public void CopyUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder oldValue, ref ValueHolder newValue) // lgtm[cs/too-many-ref-parameters]
            {
                newValue = oldValue;
            }

            /// <inheritdoc />
            public bool InPlaceUpdater(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value) // lgtm[cs/too-many-ref-parameters]
            {
                value = input;
                return true;
            }

            /// <inheritdoc />
            public void SingleReader(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                dst = value;
            }

            /// <inheritdoc />
            public void ConcurrentReader(ref KeyHolder key, ref ValueHolder input, ref ValueHolder value, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                dst = value;
            }

            /// <inheritdoc />
            public void SingleWriter(ref KeyHolder key, ref ValueHolder src, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                dst = src;
            }

            /// <inheritdoc />
            public bool ConcurrentWriter(ref KeyHolder key, ref ValueHolder src, ref ValueHolder dst) // lgtm[cs/too-many-ref-parameters]
            {
                dst = src;
                return true;
            }
        }
        // ReSharper restore RedundantAssignment
    }
}