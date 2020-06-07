using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Zeus.Handlers.Bot.Actions;
using Zeus.Localization;
using Zeus.v2;

namespace Zeus.Handlers.Bot.Abstractions
{
    public abstract class BotActionHandler<TAction> : AsyncRequestHandler<BotActionRequest<TAction>> 
        where TAction : IBotAction
    {
        protected ITelegramBotClient Bot { get; }

        protected IMessageLocalizer<BotResources> Localizer { get; }

        protected ILogger Logger { get; }

        protected BotActionHandler(ITelegramBotClient bot, IMessageLocalizer<BotResources> localizer, ILoggerFactory loggerFactory)
        {
            Bot = bot;
            Localizer = localizer;
            Logger = loggerFactory.CreateLogger(GetType());
        }
    }
}