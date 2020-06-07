using System;

namespace Zeus.Shared.Mediatr
{
    public interface IRequestHandlerFinder
    {
        Type FindHandlerType(Type requestType);
    }
}
