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
using Zeus.Handlers.Bot.Actions.Start;
using Zeus.Handlers.Bot.Actions.Subscribe;
using Zeus.Handlers.Bot.Actions.Unsubscribe;
using Zeus.Handlers.Bot.Context;
using Zeus.Handlers.Bot.Notifications;
using Zeus.Shared.Extensions;

namespace Zeus.Handlers.Bot.Updates
{
    public class BotUpdateRequestHandler : AsyncRequestHandler<BotUpdateRequest>
    {
        private readonly IMediator _mediator;
        private readonly ITelegramBotClient _botClient;
        private readonly IStringLocalizer<BotResources> _localizer;
        private readonly IBotUserProvider _botUserProvider;

        public BotUpdateRequestHandler(IMediator mediator, ITelegramBotClient botClient, IStringLocalizer<BotResources> localizer, IBotUserProvider botUserProvider)
        {
            _mediator = mediator;
            _botClient = botClient;
            _localizer = localizer;
            _botUserProvider = botUserProvider;
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
                await _mediator.Publish(new BotUnsupportedUpdateReceived(request.Update), cancellationToken);
                return;
            }

            var parser = BotActionParser.Builder()
                .WhenParsed<SubscribeAction.Format, SubscribeAction>(SendActionRequest)
                .WhenParsed<EchoAction.Format, EchoAction>(SendActionRequest)
                .WhenParsed<UnsubscribeAction.Format, UnsubscribeAction>(SendActionRequest)
                .WhenParsed<StartAction.Format, StartAction>(SendActionRequest)
                .Create();

            var bot = await _botUserProvider.GetAsync(cancellationToken);
            var message = request.Update.Message.Text.ReplaceFirst($"@{bot.Username}", string.Empty);

            var parsed = await parser.ParseAsync(message, cancellationToken);
            if (!parsed)
            {
                var replyText = _localizer.GetString(BotResources.CommandNotSupportedOrIncomplete);
                await _botClient.SendTextMessageAsync(new ChatId(request.Update.Message.Chat.Id),
                    replyText, replyToMessageId: request.Update.Message.MessageId, cancellationToken: cancellationToken);

                await _mediator.Publish(new BotUnknownCommandReceived(request.Update), cancellationToken);
            }
        }
    }
}