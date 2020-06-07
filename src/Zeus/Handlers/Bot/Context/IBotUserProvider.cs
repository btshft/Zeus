using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Zeus.Handlers.Bot.Context
{
    public interface IBotUserProvider
    {
        Task<User> GetAsync(CancellationToken cancellation = default);
    }
}