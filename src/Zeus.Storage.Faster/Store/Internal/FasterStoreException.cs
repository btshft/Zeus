using System;
using System.Runtime.Serialization;

namespace Zeus.Storage.Faster.Store.Internal
{
    [Serializable]
    public class FasterStoreException : Exception
    {
        internal FasterStoreException(string message)
            : base(message)
        {
        }

        internal FasterStoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public FasterStoreException()
        {
        }

        protected FasterStoreException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}