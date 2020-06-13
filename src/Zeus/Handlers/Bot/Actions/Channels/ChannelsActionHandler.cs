using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Localization;
using Zeus.Shared.Extensions;
using Zeus.Storage.Stores.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Channels
{
    public class ChannelsActionHandler : BotActionHandler<ChannelsAction>
    {
        private readonly IChannelStore _channelStore;
        private readonly ISubscriptionsStore _subscriptionsStore;

        public ChannelsActionHandler(
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
        protected override async Task Handle(BotActionRequest<ChannelsAction> request, CancellationToken cancellationToken)
        {
            var chatId = request.Message.Chat.Id;
            var channels = await _channelStore.GetAllAsync(cancellationToken);

            if (channels.Count < 1)
            {
                var notSubscribedText = Localizer.GetString(BotResources.ChannelsNotFound);
                await Bot.SendTextMessageAsync(new ChatId(chatId), notSubscribedText, cancellationToken: cancellationToken);
                return;
            }

            var subscribedText = Localizer.GetString(BotResources.Channels);
            var messageBuilder = new StringBuilder(subscribedText)
                .AppendLine();

            foreach (var (index, channel) in channels.Index())
            {
                var userSubscribed = await _subscriptionsStore.ExistsAsync(channel.Name, chatId, cancellationToken);

                var descriptionSuffix = string.IsNullOrWhiteSpace(channel.Description)
                    ? string.Empty
                    : $" — {channel.Description}";

                var subscribeSuffix = userSubscribed 
                    ? string.Empty 
                    : $" (/subscribe@{channel.Name})";

                messageBuilder.AppendLine($"{index + 1}. {channel.Name}{descriptionSuffix}{subscribeSuffix}");
            }

            await Bot.SendTextMessageSplitAsync(new ChatId(chatId), messageBuilder.ToString(), cancellationToken: cancellationToken);
        }
    }
}