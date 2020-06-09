using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Zeus.Shared.Mediatr
{
    public abstract class WrapExceptionsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!CanWrap(request))
                return await next();

            try
            {
                return await next();
            }
            catch (Exception e)
            {
                var resultException = Wrap(request, e);
                if (resultException != null)
                    throw resultException;

                // Throw if cant wrap
                throw;
            }
        }

        protected abstract bool CanWrap(TRequest request);

        protected abstract Exception Wrap(TRequest request, Exception source);
    }
}