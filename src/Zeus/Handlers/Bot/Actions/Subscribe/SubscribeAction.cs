using System;
using Zeus.Handlers.Bot.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Subscribe
{
    /// <summary>
    /// Subscribe to channel notifications.
    /// </summary>
    public class SubscribeAction : IBotAction
    {
        public string Channel { get; }

        public SubscribeAction(string channel)
        {
            Channel = channel;
        }

        public class Format : IBotActionFormat<SubscribeAction>
        {
            /// <inheritdoc />
            public bool TryParse(string message, out SubscribeAction action)
            {
                action = null;

                var commandParts = message.Split(' ', count: 2, StringSplitOptions.RemoveEmptyEntries);
                if (commandParts.Length < 2)
                    commandParts = message.Split('@', count: 2, StringSplitOptions.RemoveEmptyEntries);

                if (commandParts.Length != 2)
                    return false;

                var command = commandParts[0].Trim();
                var channel = commandParts[1].Trim();

                var matchedByCommand = command.Equals("/subscribe", StringComparison.InvariantCultureIgnoreCase);
                if (!matchedByCommand)
                    return false;

                action = new SubscribeAction(channel.ToLowerInvariant());
                return true;
            }
        }
    }
}
