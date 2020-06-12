using System;

namespace Zeus.Storage.Faster.Utils
{
    internal static class DisposeExtensions
    {
        public static bool TryDispose(this IDisposable disposable, out Exception exception)
        {
            exception = null;

            try
            {
                disposable.Dispose();
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }
    }
}