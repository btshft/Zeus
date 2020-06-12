using System;

namespace Zeus.Shared.Mediatr
{
    public interface IRequestHandlerFinder
    {
        Type FindHandlerTypeByRequest(Type requestType);
    }
}
