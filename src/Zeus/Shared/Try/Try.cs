using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zeus.Shared.Try
{
    public static class Try
    {
        public static Task<TryResult<T>[]> WhenAll<T>(IEnumerable<Task<T>> tasks)
        {
            var wrappedTasks = tasks.Select(
                task => task.ContinueWith(
                    t => t.IsFaulted
                        ? new TryResult<T>(t.Exception)
                        : new TryResult<T>(t.Result)));

            return Task.WhenAll(wrappedTasks);
        }

        public static async Task<TryResult> ExecuteAsync(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction();
                return TryResult.Succeed;
            }
            catch (Exception e)
            {
                return new TryResult(e);
            }
        }
    }
}