using System;

namespace Zeus.v2.Shared.Mediatr
{
    public interface IRequestHandlerFinder
    {
        Type FindHandlerType(Type requestType);
    }
}
