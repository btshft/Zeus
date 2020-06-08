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
            return Task.WhenAll(tasks.Select(async t =>
            {
                try
                {
                    var result = await t;
                    return new TryResult<T>(result);
                }
                catch (Exception e)
                {
                    return new TryResult<T>(e);
                }
            }));
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