using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;

namespace Zeus.Features.Localization
{
    public class LocalizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IOptions<LocalizationFeatureOptions> _optionsProvider;

        public LocalizationBehavior(IOptions<LocalizationFeatureOptions> optionsProvider)
        {
            _optionsProvider = optionsProvider;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var options = _optionsProvider.Value;
            var culture = new CultureInfo(options.Culture);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            return await next();
        }
    }
}
