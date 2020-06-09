using System;
using System.Runtime.Serialization;

namespace Zeus.Shared.Exceptions
{
    [Serializable]
    public class AlertManagerUpdateException : Exception
    {
        public AlertManagerUpdateException()
        {
        }

        protected AlertManagerUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public AlertManagerUpdateException(string message) : base(message)
        {
        }

        public AlertManagerUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}