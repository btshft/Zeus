using System;

namespace Zeus.Shared.Try
{
    public class TryResult
    {
        public static TryResult Succeed { get; } = new TryResult();

        public Exception Exception { get; }

        public bool IsFaulted => Exception != null;

        public TryResult()
        { }

        public TryResult(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }

    public class TryResult<TResult> : TryResult
    {
        private readonly TResult _result;

        public TResult Result
        {
            get
            {
                if (IsFaulted)
                    throw new InvalidOperationException("Unable to get result from container because it's in faulted state", Exception);

                return _result;
            }
        }

        public TryResult(TResult result)
        {
            _result = result;
        }

        public TryResult(Exception exception)
            : base(exception)
        {
        }
    }
}