using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zeus.Handlers.Alerting.Send;
using Zeus.Models.Extensions;
using Zeus.Services.Telegram;
using Zeus.Services.Templating;
using Zeus.Shared.Exceptions;
using Zeus.Shared.Mediatr;
using Zeus.Shared.Try;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Handlers.Alerting.Webhook
{
    [WrapExceptions]
    public class AlertManagerUpdateRequestHandler : AsyncRequestHandler<AlertManagerUpdateRequest>
    {
        private readonly ITemplateEngine _templateEngine;
        private readonly IChannelStore _channelStore;
        private readonly ITemplatesStore _templatesStore;
        private readonly ISubscriptionsStore _subscriptionsStore;
        private readonly IMediator _mediator;

        public AlertManagerUpdateRequestHandler(
            ITemplateEngine templateEngine, 
            IChannelStore channelStore, 
            ITemplatesStore templatesStore,
            ISubscriptionsStore subscriptionsStore, 
            IMediator mediator)
        {
            _templateEngine = templateEngine;
            _channelStore = channelStore;
            _templatesStore = templatesStore;
            _subscriptionsStore = subscriptionsStore;
            _mediator = mediator;
        }

        /// <inheritdoc />
        protected override async Task Handle(AlertManagerUpdateRequest request, CancellationToken cancellationToken)
        {
            var channel = await _channelStore.GetAsync(request.Channel, cancellationToken);
            if (channel == null)
                throw new NotFoundException($"Channel with name '{request.Channel}' not found");

            if (!channel.Enabled)
            {
                // TODO Log
                return;
            }

            var subscriptions = await _subscriptionsStore.GetAsync(channel.Name, cancellationToken);
            if (subscriptions.Count < 1)
            {
                // TODO Send dead letter
                return;
            }

            var template = await _templatesStore.GetAsync(request.Channel, cancellationToken);
            var model = new AlertsTemplateModel
            {
                Status = request.Update.Status,
                Alerts = request.Update.Alerts,
                CommonAnnotations = request.Update.CommonAnnotations,
                CommonLabels = request.Update.CommonLabels,
                GroupKey = request.Update.GroupKey,
                GroupLabels = request.Update.GroupLabels
            };

            var message = await _templateEngine.RenderAsync(template.Content, model, cancellationToken);
            var parseMode = template.Syntax.ToParseMode();
            var safeMessage = message.Escape(parseMode);

            var sendTasks = subscriptions
                .Select(s => _mediator
                    .Send(new SendAlertRequest(s, safeMessage, parseMode, request.Update), cancellationToken));

            var results = await Try.WhenAll(sendTasks);
            if (results.HasFaults())
            {
                var exceptions = results.Where(s => s.IsFaulted)
                    .Select(s => s.Exception);

                var exception = new AggregateException(exceptions);
                throw exception;
            }
        }

        public class WrapExceptionsAttribute : WrapExceptionsBaseAttribute
        {
            /// <inheritdoc />
            public override Exception Wrap(Exception source, object request)
            {
                return new AlertManagerUpdateException("Exception occured while handling alertmanager webhook request", source);
            }
        }
    }
}