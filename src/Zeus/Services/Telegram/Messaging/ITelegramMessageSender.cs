using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Zeus.Services.Telegram.Messaging.Requests;

namespace Zeus.Services.Telegram.Messaging
{
    public interface ITelegramMessageSender
    {
        Task<Message> SendAsync(TelegramMessageSendRequest request, CancellationToken cancellation = default);
    }
}
