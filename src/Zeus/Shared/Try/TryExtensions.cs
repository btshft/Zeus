using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Zeus.v2.Shared.Try
{
    public static class TryExtensions
    {
        public static bool HasFaults(this IEnumerable<TryResult> results) => results
            .Any(r => r.IsFaulted);

        [DebuggerHidden]
        public static async Task<TryResult> WhenSucceed(this Task<TryResult> result, Func<Task> continuation)
        {
            var taskResult = await result.ContinueWith(async task =>
            {
                var tryResult = task.Result;
                if (!tryResult.IsFaulted)
                {
                    await continuation();
                }

                return tryResult;
            }).Unwrap();

            return taskResult;
        }

        [DebuggerHidden]
        public static async Task<TryResult> WhenFailed(this Task<TryResult> result, Func<Task> continuation)
        {
            var taskResult = await result.ContinueWith(async task =>
            {
                var tryResult = task.Result;
                if (tryResult.IsFaulted)
                {
                    await continuation();
                }

                return tryResult;
            }).Unwrap();

            return taskResult;
        }
    }
}