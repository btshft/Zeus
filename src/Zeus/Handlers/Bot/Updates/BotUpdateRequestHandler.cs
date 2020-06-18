using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Actions;
using Zeus.Handlers.Bot.Actions.Channels;
using Zeus.Handlers.Bot.Actions.Start;
using Zeus.Handlers.Bot.Actions.Subscribe;
using Zeus.Handlers.Bot.Actions.Subscriptions;
using Zeus.Handlers.Bot.Actions.Unsubscribe;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Handlers.Bot.Context;
using Zeus.Handlers.Bot.Notifications;
using Zeus.Shared.Extensions;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Updates
{
    public class BotUpdateRequestHandler : AsyncRequestHandler<BotUpdateRequest>
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<BotResources> _localizer;
        private readonly IBotUserProvider _botUserProvider;
        private readonly ITransport<SendTelegramReply> _reply;

        public BotUpdateRequestHandler(
            IMediator mediator,
            IStringLocalizer<BotResources> localizer,
            IBotUserProvider botUserProvider, 
            ITransport<SendTelegramReply> reply)
        {
            _mediator = mediator;
            _localizer = localizer;
            _botUserProvider = botUserProvider;
            _reply = reply;
        }

        /// <inheritdoc />
        protected override async Task Handle(BotUpdateRequest request, CancellationToken cancellationToken)
        {
            async Task SendActionRequest<TAction>(TAction action, CancellationToken cancellation = default)
                where TAction : IBotAction
            {
                var actionRequest = new BotActionRequest<TAction>(request.Update, action);
                await _mediator.Send(actionRequest, cancellationToken);
            }

            if (!request.Update.IsCommand())
            {
                await _mediator.Publish(new BotReceivedUnsupportedUpdate(request.Update), cancellationToken);
                return;
            }

            var parser = BotActionParser.Builder()
                .WhenParsed<SubscribeAction.Format, SubscribeAction>(SendActionRequest)
                .WhenParsed<ChannelsAction.Format, ChannelsAction>(SendActionRequest)
                .WhenParsed<UnsubscribeAction.Format, UnsubscribeAction>(SendActionRequest)
                .WhenParsed<StartAction.Format, StartAction>(SendActionRequest)
                .WhenParsed<SubscriptionsAction.Format, SubscriptionsAction>(SendActionRequest)
                .Create();

            var bot = await _botUserProvider.GetAsync(cancellationToken);
            var message = request.Update.Message.Text.ReplaceFirst($"@{bot.Username}", string.Empty).Trim();

            var parsed = await parser.ParseAsync(message, cancellationToken);
            if (!parsed)
            {
                var replyText = _localizer.GetString(BotResources.CommandNotSupportedOrIncomplete);
                var replyRequest = new SendTelegramReply(request.Update.Message.Chat.Id, replyText)
                {
                    ReplyToMessageId = request.Update.Message.MessageId
                };

                await _reply.SendAsync(replyRequest, cancellationToken);
                await _mediator.Publish(new BotReceivedUnknownCommand(request.Update), cancellationToken);
            }
        }
    }
}