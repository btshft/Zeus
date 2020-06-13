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

                var commandParts = message.Split(' ', count: 2, StringSplitOptions.RemoveEmptyEntries);
                if (commandParts.Length < 2)
                    commandParts = message.Split('@', count: 2, StringSplitOptions.RemoveEmptyEntries);

                if (commandParts.Length != 2)
                    return false;

                var command = commandParts[0].Trim();
                var channel = commandParts[1].Trim();

                var matchedByCommand = command.Equals("/unsubscribe", StringComparison.InvariantCultureIgnoreCase);
                if (!matchedByCommand)
                    return false;

                action = new UnsubscribeAction(channel.ToLowerInvariant());
                return true;
            }
        }
    }
}
