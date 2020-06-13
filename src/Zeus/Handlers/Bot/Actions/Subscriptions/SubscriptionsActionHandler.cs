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

namespace Zeus.Handlers.Bot.Actions.Subscriptions
{
    [ReplyOnException]
    public class SubscriptionsActionHandler : BotActionHandler<SubscriptionsAction>
    {
        private readonly ISubscriptionsStore _subscriptionsStore;

        /// <inheritdoc />
        public SubscriptionsActionHandler
            (ITelegramBotClient bot, 
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
            var chatId = request.Message.Chat.Id;
            var subscriptions = await _subscriptionsStore.GetAsync(chatId, cancellationToken);

            if (subscriptions.Count < 1)
            {
                var notSubscribedText = Localizer.GetString(BotResources.ChatSubscriptionsNotFound);
                await Bot.SendTextMessageAsync(new ChatId(chatId), notSubscribedText, cancellationToken: cancellationToken);
                return;
            }

            var subscribedText = Localizer.GetString(BotResources.ChatSubscriptions);
            var messageBuilder = new StringBuilder(subscribedText)
                .AppendLine();

            foreach (var (index, subscription) in subscriptions.Index())
                messageBuilder.AppendLine($"{index + 1}. {subscription.Channel} (/unsubscribe@{subscription.Channel})");

            await Bot.SendTextMessageSplitAsync(new ChatId(chatId), messageBuilder.ToString(),
                cancellationToken: cancellationToken);
        }
    }
}