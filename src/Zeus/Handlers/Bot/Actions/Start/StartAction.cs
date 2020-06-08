using System;
using Zeus.Handlers.Bot.Abstractions;

namespace Zeus.Handlers.Bot.Actions.Start
{
    public class StartAction : IBotAction
    {
        public class Format : IBotActionFormat<StartAction>
        {
            /// <inheritdoc />
            public bool TryParse(string message, out StartAction action)
            {
                action = null;

                var isMatched = message.Equals("/start", StringComparison.InvariantCultureIgnoreCase);
                if (isMatched)
                    action = new StartAction();

                return isMatched;
            }
        }
    }
}
