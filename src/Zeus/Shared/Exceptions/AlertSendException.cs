using System;
using System.Runtime.Serialization;

namespace Zeus.Shared.Exceptions
{
    [Serializable]
    public class AlertSendException : Exception
    {
        public AlertSendException()
        {
        }

        protected AlertSendException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public AlertSendException(string message) : base(message)
        {
        }

        public AlertSendException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}