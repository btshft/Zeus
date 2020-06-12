using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Zeus.Shared.Mediatr
{
    public class WrapExceptionsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        protected IRequestHandlerFinder Finder { get; }

        public WrapExceptionsBehavior(IRequestHandlerFinder finder)
        {
            Finder = finder;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var handlerType = Finder.FindHandlerTypeByRequest(typeof(TRequest));
            var handleExceptionAttribute = handlerType.GetCustomAttributes()
                .Where(a => a is WrapExceptionsBaseAttribute)
                .Cast<WrapExceptionsBaseAttribute>()
                .FirstOrDefault();

            if (handleExceptionAttribute == null)
                return await next();

            try
            {
                return await next();
            }
            catch (Exception e)
            {
                var resultException = handleExceptionAttribute.Wrap(e, request);
                if (resultException != null)
                    ExceptionDispatchInfo.Throw(resultException);

                // Throw if cant wrap
                throw;
            }
        }
    }
}