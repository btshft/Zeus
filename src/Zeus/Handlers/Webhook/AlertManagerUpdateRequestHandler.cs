using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Telegram.Bot.Types;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;
using Zeus.v2.Models.Extensions;
using Zeus.v2.Services.Telegram.Messaging;
using Zeus.v2.Services.Telegram.Messaging.Requests;
using Zeus.v2.Services.Templating;
using Zeus.v2.Shared.Exceptions;
using Zeus.v2.Shared.Try;

namespace Zeus.v2.Handlers.Webhook
{
    public class AlertManagerUpdateRequestHandler : AsyncRequestHandler<AlertManagerUpdateRequest>
    {
        private readonly ITemplateEngine _templateEngine;
        private readonly IChannelStore _channelStore;
        private readonly ITemplatesStore _templatesStore;
        private readonly ISubscriptionsStore _subscriptionsStore;
        private readonly ITelegramMessageSender _messageSender;

        public AlertManagerUpdateRequestHandler(
            ITemplateEngine templateEngine, 
            IChannelStore channelStore, 
            ITemplatesStore templatesStore,
            ISubscriptionsStore subscriptionsStore,
            ITelegramMessageSender messageSender)
        {
            _templateEngine = templateEngine;
            _channelStore = channelStore;
            _templatesStore = templatesStore;
            _subscriptionsStore = subscriptionsStore;
            _messageSender = messageSender;
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

            var sendTasks = subscriptions
                .Select(s => _messageSender.SendAsync(
                    new TelegramMessageSendRequest(new ChatId(s.ChatId), message)
                    {
                        ParseMode = template.Syntax.ToParseMode(),
                        DisableNotification = s.DisableNotifications
                    }, cancellationToken));

            var results = await Try.WhenAll(sendTasks);
            if (results.HasFaults())
            {
                var exception = new AggregateException(results.Select(s => s.Exception));
                throw exception;
            }
        }
    }
}