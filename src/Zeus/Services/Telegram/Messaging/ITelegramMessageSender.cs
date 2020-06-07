using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Zeus.v2.Services.Telegram.Messaging.Requests;

namespace Zeus.v2.Services.Telegram.Messaging
{
    public interface ITelegramMessageSender
    {
        Task<Message> SendAsync(TelegramMessageSendRequest request, CancellationToken cancellation = default);
    }
}
