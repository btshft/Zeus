using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot;
using Zeus.Alerting.Routing;
using Zeus.Alerting.Templating;
using Zeus.Templating.Abstraction;

namespace Zeus.Alerting.Requests.Handlers
{
    public class AlertManagerUpdateHandler : AsyncRequestHandler<ProcessAlertManagerUpdate>
    {
        private readonly ITelegramBotClient _bot;
        private readonly ITemplateEngine _templateEngine;
        private readonly IAlertRoutesResolver _routesResolver;
        private readonly ITemplateResolver _templateResolver;

        public AlertManagerUpdateHandler(ITelegramBotClient bot, ITemplateEngine templateEngine, IAlertRoutesResolver routesResolver, ITemplateResolver templateResolver)
        {
            _bot = bot;
            _templateEngine = templateEngine;
            _routesResolver = routesResolver;
            _templateResolver = templateResolver;
        }

        protected override async Task Handle(ProcessAlertManagerUpdate request, CancellationToken cancellationToken)
        {
            var alertRoutes = await _routesResolver.ResolveAsync(request.Update, cancellationToken);
            if (alertRoutes.Routes.Count == 0)
            {
                // TODO: Send dead letter alert notification
                return;
            }
            foreach (var route in alertRoutes.Routes)
            {
                var template = await _templateResolver.ResolveAsync(route, cancellationToken);
                var message = await _templateEngine.RenderFileAsync(template.Path, new
                {
                    route.Alerts,
                    request.Update.Status,
                    request.Update.CommonAnnotations,
                    request.Update.CommonLabels,
                    request.Update.ExternalUrl,
                    request.Update.GroupLabels,
                    request.Update.GroupKey
                }, cancellationToken);

                var parseMode = template.RenderMode.ToParseMode();

                // TODO: Handle text size limitations
                await _bot.SendTextMessageAsync(route.Destination.Id, message, disableWebPagePreview: true, parseMode: parseMode, cancellationToken: cancellationToken);
            }
        }
    }
}
