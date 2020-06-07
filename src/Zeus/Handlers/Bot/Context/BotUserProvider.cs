using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Context
{
    public class BotUserProvider : IBotUserProvider
    {
        private readonly Lazy<Task<User>> _botUserProvider;

        public BotUserProvider(ITelegramBotClient client)
        {
            _botUserProvider = new Lazy<Task<User>>(async () => await client.GetMeAsync());
        }


        public async Task<User> GetAsync(CancellationToken cancellation = default)
        {
            return await _botUserProvider.Value;
        }
    }
}