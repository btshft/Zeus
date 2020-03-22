using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Zeus.Bot.State
{
    public class BotState : IBotState
    {
        public Guid Id { get; set; }
        public ICollection<Chat> Conversations { get; set; }
    }
}
