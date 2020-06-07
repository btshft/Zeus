using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StackExchange.Profiling;

namespace Zeus.v2.Features.Profiling.Behaviors
{
    public class ProfilingBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    {
        /// <inheritdoc />
        public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            var profiler = MiniProfiler.Current;
            if (profiler == null)
                return await next();

            var handlerName = TypeNameHelper.GetTypeDisplayName(typeof(TRequest), fullName: false, includeGenericParameterNames: true);
            using (profiler.Step($"Handler: {handlerName}"))
            {
                return await next();
            }
        }
    }
}