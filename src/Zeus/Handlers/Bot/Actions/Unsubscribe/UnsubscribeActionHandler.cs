using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Reply;
using Zeus.Localization;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Unsubscribe
{
    [AllowReply]
    public class UnsubscribeActionHandler : BotActionHandler<UnsubscribeAction>
    {
        private readonly IChannelStore _channelStore;
        private readonly ISubscriptionsStore _subscriptionsStore;

        public UnsubscribeActionHandler(
            ITelegramBotClient bot, 
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory, 
            IChannelStore channelStore, 
            ISubscriptionsStore subscriptionsStore) 
            : base(bot, localizer, loggerFactory)
        {
            _channelStore = channelStore;
            _subscriptionsStore = subscriptionsStore;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<UnsubscribeAction> request, CancellationToken cancellationToken)
        {
            var chat = request.Update.Message.Chat;
            var messageId = request.Update.Message.MessageId;

            var channel = await _channelStore.GetAsync(request.Action.Channel, cancellationToken);
            if (channel == null)
            {
                var message = Localizer.GetString(BotResources.UnsubscribeFailedChannelNotFound);
                await Bot.SendTextMessageAsync(new ChatId(chat.Id), message, replyToMessageId: messageId,
                    cancellationToken: cancellationToken);

                return;
            }

            var existingSubscription = await _subscriptionsStore.GetAsync(chat.Id, request.Action.Channel, cancellationToken);
            if (existingSubscription == null)
            {
                var message = Localizer.GetString(BotResources.UnsubscribeFailedNotSubscribed);
                await Bot.SendTextMessageAsync(new ChatId(chat.Id), message, replyToMessageId: messageId,
                    cancellationToken: cancellationToken);

                return;
            }

            await _subscriptionsStore.RemoveAsync(existingSubscription, cancellationToken);

            var replyMessage = Localizer.GetString(BotResources.UnsubscribeSucceed);
            await Bot.SendTextMessageAsync(new ChatId(chat.Id), replyMessage,
                cancellationToken: cancellationToken);
        }
    }
}