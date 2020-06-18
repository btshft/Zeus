using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Localization;
using Zeus.Shared.Extensions;
using Zeus.Storage.Stores.Abstractions;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Actions.Subscriptions
{
    public class SubscriptionsActionHandler : BotActionHandler<SubscriptionsAction>
    {
        private readonly ISubscriptionsStore _subscriptionsStore;

        /// <inheritdoc />
        public SubscriptionsActionHandler(
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory, 
            ITransport<SendTelegramReply> reply, 
            ISubscriptionsStore subscriptionsStore) : base(localizer, loggerFactory, reply)
        {
            _subscriptionsStore = subscriptionsStore;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<SubscriptionsAction> request, CancellationToken cancellationToken)
        {
            var chatId = request.Message.Chat.Id;
            var subscriptions = await _subscriptionsStore.GetAsync(chatId, cancellationToken);

            if (subscriptions.Count < 1)
            {
                var notSubscribedText = Localizer.GetString(BotResources.ChatSubscriptionsNotFound);
                await Reply.SendAsync(new SendTelegramReply(chatId, notSubscribedText), cancellationToken);

                return;
            }

            var subscribedText = Localizer.GetString(BotResources.ChatSubscriptions);
            var messageBuilder = new StringBuilder(subscribedText)
                .AppendLines(count: 2);

            foreach (var (index, subscription) in subscriptions.Index())
            {
                var command = $"/unsubscribe_{subscription.Channel.Replace('-', '_')}";
                messageBuilder.AppendLine($"{index + 1}. {subscription.Channel} ({command})");
            }

            await Reply.SendAsync(new SendTelegramReply(chatId, messageBuilder.ToString()),
                cancellationToken);
        }
    }
}