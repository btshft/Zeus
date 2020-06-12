using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Zeus.Storage.Faster.Options;
using Zeus.Storage.Faster.Serialization;
using Zeus.Storage.Faster.Utils;

namespace Zeus.Storage.Faster.Store.Internal
{
    internal partial class FasterStore<TKey, TValue> : IDisposable, IAsyncEnumerable<KeyValuePair<TKey, TValue>>
    {
        private const long StoreSize = 1L << 20;

        private readonly FasterKV<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, StoreFunctions> _keyValueStore;
        private readonly ClientSession<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, StoreFunctions> _session;
        private readonly IDevice _indexDevice;
        private readonly IDevice _objectDevice;
        private readonly SerializerSettings<KeyHolder, ValueHolder> _serializerSettings;
        private readonly string _checkpointsPath;

        private readonly ILogger<FasterStore<TKey, TValue>> _logger;

        public FasterStore(IOptions<FasterStoreOptions> optionsProvider, IFasterEqualityComparer<TKey> keyComparer, ILogger<FasterStore<TKey, TValue>> logger)
        {
            _logger = logger;
            var options = optionsProvider.Value;
            var indexStorePath = Path.Combine(options.Directory, $"{options.Name}-index.log");
            var objectStorePath = Path.Combine(options.Directory, $"{options.Name}-object.log");

            _indexDevice = Devices.CreateLogDevice(indexStorePath,
                preallocateFile: false, deleteOnClose: false, recoverDevice: true);

            _objectDevice = Devices.CreateLogDevice(objectStorePath,
                preallocateFile: false, deleteOnClose: false, recoverDevice: true);

            _checkpointsPath = Path.Combine(options.Directory, "checkpoints");

            if (!Directory.Exists(_checkpointsPath))
            {
                _logger.LogInformation("Checkpoints directory not exists. Creating...");
                Directory.CreateDirectory(_checkpointsPath);
            }

            var logSettings = new LogSettings
            {
                LogDevice = _indexDevice,
                ObjectLogDevice = _objectDevice,
                MemorySizeBits = (int)options.MemorySize,
                PageSizeBits = (int)options.PageSize,
                SegmentSizeBits = (int)options.SegmentSize,
                ReadCacheSettings = new ReadCacheSettings()
                {
                    MemorySizeBits = (int)options.MemorySize + 1,
                    PageSizeBits = (int)options.PageSize + 1,
                    SecondChanceFraction = .2
                }
            };

            _serializerSettings = new SerializerSettings<KeyHolder, ValueHolder>
            {
                keySerializer = () => new KeySerializer(options.JsonOptions),
                valueSerializer = () => new ValueSerializer(options.JsonOptions)
            };

            var comparer = new KeyComparerAdapter(keyComparer);
            var functions = new StoreFunctions(_logger);
            var checkpointsSettings = new CheckpointSettings
            {
                CheckpointDir = _checkpointsPath,
                CheckPointType = CheckpointType.Snapshot
            };

            _keyValueStore = new FasterKV<KeyHolder, ValueHolder, ValueHolder, ValueHolder, StoreContext, StoreFunctions>(
                StoreSize, functions, logSettings, checkpointsSettings, _serializerSettings, comparer);

            var checkpoints = Directory.GetDirectories(_checkpointsPath).Length;
            if (checkpoints > 0)
            {
                _logger.LogInformation($"Found {checkpoints} checkpoints. Recovering store...");
                _keyValueStore.Recover();
                _logger.LogInformation("Store recovered from checkpoints");
            }

            _session = _keyValueStore.NewSession();
        }

        public async ValueTask<(bool exists, TValue value)> GetAsync(TKey key,
            CancellationToken cancellation = default)
        {
            var keyHolder = new KeyHolder(key);
            var inputHolder = default(ValueHolder);

            try
            {
                var readResult = await _session.ReadAsync(ref keyHolder, ref inputHolder, StoreContext.Null, cancellation);
                var (status, valueHolder) = readResult.CompleteRead();
                if (status == Status.ERROR)
                    throw new FasterStoreException(message: $"Read '{key}' error");

                return status == Status.OK
                    ? (true, valueHolder.Value)
                    : (false, default(TValue));
            }
            catch (FasterStoreException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FasterStoreException($"Exception while reading key '{key}'", e);
            }
        }

        public async ValueTask StoreAsync(TKey key, TValue value, CancellationToken cancellation = default)
        {
            var keyHolder = new KeyHolder(key);
            var valueHolder = new ValueHolder(value);

            try
            {
                await _session.UpsertAsync(ref keyHolder, ref valueHolder, StoreContext.Null, token: cancellation);
            }
            catch (FasterStoreException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FasterStoreException($"Exception while storing key '{key}'", e);
            }
        }

        public async Task RemoveAsync(TKey key, CancellationToken cancellation = default)
        {
            var keyHolder = new KeyHolder(key);

            try
            {
                await _session.DeleteAsync(ref keyHolder, StoreContext.Null, token: cancellation);
            }
            catch (FasterStoreException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FasterStoreException($"Exception while storing key '{key}'", e);
            }
        }

        public Task CommitAsync(CancellationToken cancellation = default)
        {
            async Task CleanupAsync(Guid checkpointToken)
            {
                const string indexCheckpoints = "index-checkpoints";
                const string cprCheckpoints = "cpr-checkpoints";

                await _keyValueStore.CompleteCheckpointAsync(cancellation);

                try
                {
                    if (!Directory.Exists(_checkpointsPath))
                        return;

                    var previousSnapshots = Directory.GetDirectories(Path.Combine(_checkpointsPath, indexCheckpoints))
                        .Select(Path.GetFileName)
                        .Where(f => f != checkpointToken.ToString())
                        .ToArray();

                    if (previousSnapshots.Length < 1)
                        return;

                    _logger.LogInformation($"{previousSnapshots.Length} old checkpoint will be removed");

                    foreach (var snapshot in previousSnapshots)
                    {
                        var indexCheckpointsPath = Path.Combine(_checkpointsPath, indexCheckpoints, snapshot);
                        var cprCheckpointsPath = Path.Combine(_checkpointsPath, cprCheckpoints, snapshot);

                        _logger.LogInformation($"Removing '{indexCheckpointsPath}' checkpoint");
                        Directory.Delete(indexCheckpointsPath, recursive: true);

                        _logger.LogInformation($"Removing '{cprCheckpointsPath}' checkpoint");
                        Directory.Delete(cprCheckpointsPath, recursive: true);
                    }

                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, $"Failed to perform cleanup. Latest checkpoint is '{checkpointToken}'");
                }
            }

            var policy = Policy
                .HandleResult<Guid?>(r => !r.HasValue)
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                });

            var checkpoint = policy.Execute(() =>
            {
                cancellation.ThrowIfCancellationRequested();
                return _keyValueStore.TakeFullCheckpoint(out var token) 
                    ? token : (Guid?) null;
            });

            return Task.Run(async () =>
            {
                if (!checkpoint.HasValue)
                    throw new FasterStoreException("Unable to commit changes, device is busy");

                await CleanupAsync(checkpoint.Value);

            }, cancellation);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            try
            {
                _indexDevice.Close();
                _objectDevice.Close();
                _session.Dispose();
                _keyValueStore.Dispose();
            }
            catch (Exception)
            {
                _session.TryDispose(out _);
                _keyValueStore.TryDispose(out _);
            }
        }

        /// <inheritdoc />
        public IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new FasterAsyncEnumerator(this);
        }
    }
}