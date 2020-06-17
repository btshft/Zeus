using System.Threading;

namespace Zeus.Handlers.Bot.Context
{
    public class BotActionContextAccessor : IBotActionContextAccessor
    {
        private static readonly AsyncLocal<BotContextHolder> CurrentContext
            = new AsyncLocal<BotContextHolder>();

        public BotActionContext Context
        {
            get => CurrentContext.Value?.Context;
            set
            {
                var holder = CurrentContext.Value;
                if (holder != null)
                {
                    holder.Context = null;
                }

                if (value != null)
                {
                    CurrentContext.Value = new BotContextHolder
                    {
                        Context = value
                    };
                }
            }
        }

        private class BotContextHolder
        {
            public BotActionContext Context;
        }
    }
}