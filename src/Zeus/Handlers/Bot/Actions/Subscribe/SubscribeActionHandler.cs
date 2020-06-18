using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Localization;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Actions.Subscribe
{
    public class SubscribeActionHandler : BotActionHandler<SubscribeAction>
    {
        private readonly ISubscriptionsStore _subscriptionsStore;
        private readonly IChannelStore _channelStore;

        /// <inheritdoc />
        public SubscribeActionHandler(
            IMessageLocalizer<BotResources> localizer,
            ILoggerFactory loggerFactory,
            ITransport<SendTelegramReply> reply, 
            ISubscriptionsStore subscriptionsStore, 
            IChannelStore channelStore) : base(localizer, loggerFactory, reply)
        {
            _subscriptionsStore = subscriptionsStore;
            _channelStore = channelStore;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<SubscribeAction> request, CancellationToken cancellationToken)
        {
            var chat = request.Update.Message.Chat;

            var channel = await _channelStore.GetAsync(request.Action.Channel, cancellationToken);
            if (channel == null)
            {
                var message = Localizer.GetString(BotResources.SubscribeFailedChannelNotFound);
                await Reply.SendAsync(new SendTelegramReply(chat.Id, message),
                    cancellationToken);

                return;
            }

            var subscriptionExists = await _subscriptionsStore.ExistsAsync(request.Action.Channel, chat.Id, cancellationToken);
            if (subscriptionExists)
            {
                var message = Localizer.GetString(BotResources.SubscribeFailedAlreadySubscribed);
                await Reply.SendAsync(new SendTelegramReply(chat.Id, message), cancellationToken);

                return;
            }

            var subscription = new AlertsSubscription
            {
                Channel = request.Action.Channel,
                ChatId = chat.Id,
                ChatName = chat.Username != null
                    ? $"@{chat.Username}"
                    : chat.Title
            };

            await _subscriptionsStore.StoreAsync(subscription, cancellationToken);

            var replyMessage = Localizer.GetString(BotResources.SubscribeSucceed);
            await Reply.SendAsync(new SendTelegramReply(chat.Id, replyMessage), cancellationToken);
        }
    }
}