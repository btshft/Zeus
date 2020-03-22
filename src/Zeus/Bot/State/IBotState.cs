using System.Collections.Generic;
using Telegram.Bot.Types;
using Zeus.Storage.Abstraction;

namespace Zeus.Bot.State
{
    public interface IBotState : IIdentifiable
    {
        ICollection<Chat> Conversations { get; set; }
    }
}