using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Zeus.Alerting.Options;
using Zeus.Alerting.Routing;

namespace Zeus.Alerting.Templating
{
    public class TemplateResolver : ITemplateResolver
    {
        private readonly IOptions<AlertingOptions> _optionsProvider;

        public TemplateResolver(IOptions<AlertingOptions> optionsProvider)
        {
            _optionsProvider = optionsProvider;
        }

        /// <inheritdoc />
        public Task<AlertTemplate> ResolveAsync(AlertRoute route, CancellationToken cancellation = default)
        {
            var templatingOptions = _optionsProvider.Value.Templating;
            var renderMode = templatingOptions.Default.RenderMode;
            var templatePath = templatingOptions.Default.TemplatePath;

            if (templatingOptions.Overrides != null)
            {
                foreach (var templateOptions in templatingOptions.Overrides)
                {
                    if (templateOptions.Selector == null)
                        continue;

                    if (string.Equals(templateOptions.Selector.RouteName, route.Name))
                    {
                        templatePath = templateOptions.TemplatePath;
                        renderMode = templateOptions.RenderMode;
                        // No reason to continue because template found
                        break;
                    }

                    if (templateOptions.Selector.ChatId == route.Destination.Id)
                    {
                        templatePath = templateOptions.TemplatePath;
                        renderMode = templateOptions.RenderMode;
                        // continue iterations because
                        // we still can find more precise template matched by route
                    }
                }
            }

            return Task.FromResult(new AlertTemplate(renderMode, templatePath));
        }
    }
}