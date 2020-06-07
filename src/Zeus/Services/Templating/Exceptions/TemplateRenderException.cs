using System;
using System.Runtime.Serialization;

namespace Zeus.v2.Services.Templating.Exceptions
{
    [Serializable]
    public class TemplateRenderException : Exception
    {
        public TemplateRenderException()
        {
        }

        protected TemplateRenderException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        public TemplateRenderException(string message) : base(message)
        {
        }

        public TemplateRenderException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}