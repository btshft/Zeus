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

namespace Zeus.Handlers.Bot.Actions.Channels
{
    public class ChannelsActionHandler : BotActionHandler<ChannelsAction>
    {
        private readonly IChannelStore _channelStore;
        private readonly ISubscriptionsStore _subscriptionsStore;

        /// <inheritdoc />
        public ChannelsActionHandler(
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory, 
            ITransport<SendTelegramReply> reply, 
            IChannelStore channelStore,
            ISubscriptionsStore subscriptionsStore) : base(localizer, loggerFactory, reply)
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
                var replyRequest = new SendTelegramReply(chatId, notSubscribedText);

                await Reply.SendAsync(replyRequest, cancellationToken);
                return;
            }

            var subscribedText = Localizer.GetString(BotResources.Channels);
            var messageBuilder = new StringBuilder(subscribedText)
                .AppendLines(count: 2);

            foreach (var (index, channel) in channels.Index())
            {
                var userSubscribed = await _subscriptionsStore.ExistsAsync(channel.Name, chatId, cancellationToken);

                var descriptionSuffix = string.IsNullOrWhiteSpace(channel.Description)
                    ? string.Empty
                    : $" — {channel.Description}";

                var subscribeSuffix = userSubscribed 
                    ? string.Empty 
                    : $" (/subscribe_{channel.Name.Replace('-', '_')})";

                messageBuilder.AppendLine($"{index + 1}. {channel.Name}{descriptionSuffix}{subscribeSuffix}");
            }

            await Reply.SendAsync(new SendTelegramReply(chatId, messageBuilder.ToString()), cancellationToken);
        }
    }
}