using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Reply;
using Zeus.Localization;
using Zeus.Shared.Extensions;
using Zeus.Storage.Stores.Abstractions;
using Zeus.v2;

namespace Zeus.Handlers.Bot.Actions.Subscriptions
{
    [AllowReply]
    public class SubscriptionsActionHandler : BotActionHandler<SubscriptionsAction>
    {
        private readonly ISubscriptionsStore _subscriptionsStore;

        public SubscriptionsActionHandler(
            ITelegramBotClient bot, 
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory, 
            ISubscriptionsStore subscriptionsStore) 
            : base(bot, localizer, loggerFactory)
        {
            _subscriptionsStore = subscriptionsStore;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotActionRequest<SubscriptionsAction> request, CancellationToken cancellationToken)
        {
            var chat = request.Update.Message.Chat;
            var subscriptions = await _subscriptionsStore.GetAllAsync(cancellationToken);

            if (!subscriptions.Any())
            {
                var notFoundMessage = Localizer.GetString(BotResources.SubscriptionsNotFound);
                await Bot.SendTextMessageAsync(new ChatId(chat.Id), notFoundMessage.ToString(),
                    cancellationToken: cancellationToken);

                return;
            }

            var subscriptionsByChannel = subscriptions.GroupBy(s => s.Channel);
            var subscriptionsMessageBuilder = new StringBuilder();

            foreach (var group in subscriptionsByChannel)
            {
                var channelSubscribersMessage = Localizer.GetString(BotResources.SubscriptionsChannelSubscribers, group.Key);
                subscriptionsMessageBuilder.AppendLine(channelSubscribersMessage);

                foreach (var (index, subscription) in group.Index())
                {
                    subscriptionsMessageBuilder.AppendLine($"{index + 1}. {subscription.ChatName}");
                }

                subscriptionsMessageBuilder.AppendLine();
            }

            await Bot.SendTextMessageAsync(new ChatId(chat.Id), subscriptionsMessageBuilder.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}