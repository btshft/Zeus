using System;

namespace Zeus.Shared.Mediatr
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class WrapExceptionsBaseAttribute : Attribute
    {
        public abstract Exception Wrap(Exception source, object request);
    }
}