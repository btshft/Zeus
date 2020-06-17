using MediatR;
using Microsoft.Extensions.Logging;
using Zeus.Handlers.Bot.Actions;
using Zeus.Handlers.Bot.Consumers;
using Zeus.Localization;
using Zeus.Transport;

namespace Zeus.Handlers.Bot.Abstractions
{
    public abstract class BotActionHandler<TAction> : AsyncRequestHandler<BotActionRequest<TAction>> 
        where TAction : IBotAction
    {
        protected ITransport<SendTelegramReply> Reply { get; }

        protected IMessageLocalizer<BotResources> Localizer { get; }

        protected ILogger Logger { get; }

        protected BotActionHandler(
            IMessageLocalizer<BotResources> localizer, 
            ILoggerFactory loggerFactory, 
            ITransport<SendTelegramReply> reply)
        {
            Localizer = localizer;
            Reply = reply;
            Logger = loggerFactory.CreateLogger(GetType());
        }
    }
}