using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zeus.Shared.Threading
{
    public class ResourceLocker
    {
        private static readonly Dictionary<string, Holder> Semaphores
            = new Dictionary<string, Holder>();

        public static async Task<IDisposable> LockAsync(string name, CancellationToken cancellation = default)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            var semaphore = ProvideSemaphore(name);
            await semaphore.WaitAsync(cancellation);

            return new Releaser(name);
        }

        private static SemaphoreSlim ProvideSemaphore(string key)
        {
            Holder holder;
            lock (Semaphores)
            {
                if (Semaphores.TryGetValue(key, out holder))
                {
                    ++holder.References;
                }
                else
                {
                    holder = new Holder(new SemaphoreSlim(1, 1));
                    Semaphores[key] = holder;
                }
            }
            return holder.Semaphore;
        }

        private sealed class Holder
        {
            public Holder(SemaphoreSlim semaphore)
            {
                References = 1;
                Semaphore = semaphore;
            }

            public int References { get; set; }

            public SemaphoreSlim Semaphore { get; }
        }

        private sealed class Releaser : IDisposable
        {
            internal string Key { get;}

            internal Releaser(string key)
            {
                Key = key;
            }

            public void Dispose()
            {
                Holder holder = null;
                lock (Semaphores)
                {
                    holder = Semaphores[Key];
                    --holder.References;

                    if (holder.References == 0)
                        Semaphores.Remove(Key);
                }

                holder.Semaphore.Release();
            }
        }
    }
}
