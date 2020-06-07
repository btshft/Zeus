using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Reply;
using Zeus.Localization;
using Zeus.Storage.Models.Alerts;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Subscribe
{
    [AllowReply]
    public class SubscribeActionHandler : BotActionHandler<SubscribeAction>
    {
        private readonly ISubscriptionsStore _subscriptionsStore;
        private readonly IChannelStore _channelStore;

        public SubscribeActionHandler(
            ITelegramBotClient bot,
            IMessageLocalizer<BotResources> localizer,
            ILoggerFactory loggerFactory, 
            ISubscriptionsStore subscriptionsStore,
            IChannelStore channelStore) 
            : base(bot, localizer, loggerFactory)
        {
            _subscriptionsStore = subscriptionsStore;
            _channelStore = channelStore;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<SubscribeAction> request, CancellationToken cancellationToken)
        {
            var chat = request.Update.Message.Chat;
            var messageId = request.Update.Message.MessageId;

            var channel = await _channelStore.GetAsync(request.Action.Channel, cancellationToken);
            if (channel == null)
            {
                var message = Localizer.GetString(BotResources.SubscribeFailedChannelNotFound);
                await Bot.SendTextMessageAsync(new ChatId(chat.Id), message, replyToMessageId: messageId,
                    cancellationToken: cancellationToken);

                return;
            }

            var subscriptionExists = await _subscriptionsStore.ExistsAsync(request.Action.Channel, chat.Id, cancellationToken);
            if (subscriptionExists)
            {
                var message = Localizer.GetString(BotResources.SubscribeFailedAlreadySubscribed);
                await Bot.SendTextMessageAsync(new ChatId(chat.Id), message, replyToMessageId: messageId,
                    cancellationToken: cancellationToken);

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
            await Bot.SendTextMessageAsync(new ChatId(chat.Id), replyMessage,
                cancellationToken: cancellationToken);
        }
    }
}