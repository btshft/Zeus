using System;
using Zeus.Shared.Exceptions;
using Zeus.Shared.Mediatr;

namespace Zeus.Handlers.Webhook
{
    public class WrapAlertManagerUpdateExceptionsBehavior<TRequest, TResponse> 
        : WrapExceptionsBehavior<TRequest, TResponse>
    {
        /// <inheritdoc />
        protected override bool CanWrap(TRequest request)
        {
            return typeof(TRequest) == typeof(AlertManagerUpdateRequest);
        }

        /// <inheritdoc />
        protected override Exception Wrap(TRequest request, Exception source)
        {
            return new AlertManagerUpdateException("Exception occured while handling alertmanager webhook request", source);
        }
    }
}