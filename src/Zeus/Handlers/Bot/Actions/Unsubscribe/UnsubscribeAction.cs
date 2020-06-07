using System;
using Zeus.Handlers.Bot.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Unsubscribe
{
    public class UnsubscribeAction : IBotAction
    {
        public string Channel { get; }

        public UnsubscribeAction(string channel)
        {
            Channel = channel;
        }

        public class Format : IBotActionFormat<UnsubscribeAction>
        {
            /// <inheritdoc />
            public bool TryParse(string message, out UnsubscribeAction action)
            {
                action = null;

                var matchedByStart = message.StartsWith("/unsubscribe", StringComparison.InvariantCultureIgnoreCase);
                if (!matchedByStart)
                    return false;

                var commandParts = message.Split(' ', count: 2, StringSplitOptions.RemoveEmptyEntries);
                if (commandParts.Length < 2)
                    return false;

                var channel = commandParts[1].Trim();

                action = new UnsubscribeAction(channel);
                return true;
            }
        }
    }
}
