using System;

namespace Zeus.Storage.Abstraction
{
    public interface IIdentifiable
    {
        Guid Id { get; }
    }
}