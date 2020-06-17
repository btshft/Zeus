using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Localization;
using Zeus.Storage.Stores.Abstractions;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Actions.Unsubscribe
{
    public class UnsubscribeActionHandler : BotActionHandler<UnsubscribeAction>
    {
        private readonly IChannelStore _channelStore;
        private readonly ISubscriptionsStore _subscriptionsStore;

        /// <inheritdoc />
        public UnsubscribeActionHandler(
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory,
            ITransport<SendTelegramReply> reply, IChannelStore channelStore, ISubscriptionsStore subscriptionsStore) 
            : base(localizer, loggerFactory, reply)
        {
            _channelStore = channelStore;
            _subscriptionsStore = subscriptionsStore;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<UnsubscribeAction> request, CancellationToken cancellationToken)
        {
            var chat = request.Update.Message.Chat;

            var channel = await _channelStore.GetAsync(request.Action.Channel, cancellationToken);
            if (channel == null)
            {
                var message = Localizer.GetString(BotResources.UnsubscribeFailedChannelNotFound);
                await Reply.SendAsync(new SendTelegramReply(chat.Id, message), cancellationToken);

                return;
            }

            var existingSubscription = await _subscriptionsStore.GetAsync(chat.Id, request.Action.Channel, cancellationToken);
            if (existingSubscription == null)
            {
                var message = Localizer.GetString(BotResources.UnsubscribeFailedNotSubscribed);
                await Reply.SendAsync(new SendTelegramReply(chat.Id, message), cancellationToken);

                return;
            }

            await _subscriptionsStore.RemoveAsync(existingSubscription, cancellationToken);

            var replyMessage = Localizer.GetString(BotResources.UnsubscribeSucceed);
            await Reply.SendAsync(new SendTelegramReply(chat.Id, replyMessage), cancellationToken);
        }
    }
}