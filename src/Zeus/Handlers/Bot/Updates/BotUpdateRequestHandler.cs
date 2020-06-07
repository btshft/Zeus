using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Zeus.Handlers.Bot.Abstractions;
using Zeus.Handlers.Bot.Actions;
using Zeus.Handlers.Bot.Actions.Echo;
using Zeus.Handlers.Bot.Actions.Subscribe;
using Zeus.Handlers.Bot.Actions.Subscriptions;
using Zeus.Handlers.Bot.Actions.Unsubscribe;
using Zeus.Handlers.Bot.Notifications;
using Zeus.v2;

namespace Zeus.Handlers.Bot.Updates
{
    public class BotUpdateRequestHandler : AsyncRequestHandler<BotUpdateRequest>
    {
        private readonly IMediator _mediator;
        private readonly ITelegramBotClient _botClient;
        private readonly IStringLocalizer<BotResources> _localizer;

        public BotUpdateRequestHandler(IMediator mediator, ITelegramBotClient botClient, IStringLocalizer<BotResources> localizer)
        {
            _mediator = mediator;
            _botClient = botClient;
            _localizer = localizer;
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

            if (request.Update.Type != UpdateType.Message)
            {
                await _mediator.Publish(new BotUnsupportedUpdateReceived(request.Update), cancellationToken);
                return;
            }

            var parser = BotActionParser.Builder()
                .WhenParsed<SubscribeAction.Format, SubscribeAction>(SendActionRequest)
                .WhenParsed<EchoAction.Format, EchoAction>(SendActionRequest)
                .WhenParsed<SubscriptionsAction.Format, SubscriptionsAction>(SendActionRequest)
                .WhenParsed<UnsubscribeAction.Format, UnsubscribeAction>(SendActionRequest)
                .Create();

            var parsed = await parser.ParseAsync(request.Update.Message.Text, cancellationToken);
            if (!parsed)
            {
                var message = _localizer.GetString(BotResources.CommandNotSupportedOrIncomplete);
                await _botClient.SendTextMessageAsync(new ChatId(request.Update.Message.Chat.Id),
                    message, replyToMessageId: request.Update.Message.MessageId, cancellationToken: cancellationToken);

                await _mediator.Publish(new BotUnknownCommandReceived(request.Update), cancellationToken);
            }
        }
    }
}