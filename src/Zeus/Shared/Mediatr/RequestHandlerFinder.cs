using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Zeus.Shared.Mediatr
{
    public class RequestHandlerFinder : IRequestHandlerFinder
    {
        private static readonly ConcurrentDictionary<Type, Type> TypeCache
            = new ConcurrentDictionary<Type, Type>();

        private readonly IServiceCollection _serviceCollection;

        public RequestHandlerFinder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public Type FindHandlerType<TRequest>() 
            where TRequest : IRequest
        {
            return TypeCache.GetOrAdd(typeof(TRequest), t =>
            {
                var descriptor = _serviceCollection
                    .FirstOrDefault(s => s.ServiceType == typeof(IRequestHandler<TRequest>));

                return descriptor?.ImplementationType;
            });
        }

        /// <inheritdoc />
        public Type FindHandlerType(Type requestType)
        {
            if (requestType == null)
                throw new ArgumentNullException(nameof(requestType));

            if (!typeof(IBaseRequest).IsAssignableFrom(requestType))
                throw new ArgumentException($"Type '{requestType.FullName}' should implement IRequest", nameof(requestType));

            return TypeCache.GetOrAdd(requestType, t =>
            {
                Type GetHandlerType()
                {
                    var requestInterface = requestType.GetTypeInfo().ImplementedInterfaces
                        .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

                    var responseType = requestInterface.GetGenericArguments()[0];
                    return typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
                }

                var handlerType = GetHandlerType();
                var descriptor = _serviceCollection
                    .FirstOrDefault(s => s.ServiceType == handlerType);

                return descriptor?.ImplementationType;
            });
        }
    }
}