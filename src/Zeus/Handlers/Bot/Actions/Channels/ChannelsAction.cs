using System;
using Zeus.Handlers.Bot.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Channels
{
    public class ChannelsAction : IBotAction
    {
        public class Format : IBotActionFormat<ChannelsAction>
        {
            /// <inheritdoc />
            public bool TryParse(string message, out ChannelsAction action)
            {
                action = null;

                var isMatched = message.Equals("/channels", StringComparison.InvariantCultureIgnoreCase);
                if (isMatched)
                    action = new ChannelsAction();

                return isMatched;
            }
        }
    }
}