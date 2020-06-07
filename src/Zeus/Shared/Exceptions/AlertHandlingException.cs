using System;
using System.Runtime.Serialization;

namespace Zeus.Shared.Exceptions
{
    [Serializable]
    public class AlertHandlingException : Exception
    {
        public AlertHandlingException()
        {
        }

        protected AlertHandlingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public AlertHandlingException(string message) : base(message)
        {
        }

        public AlertHandlingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}