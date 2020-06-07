using System;
using Zeus.v2.Handlers.Bot.Abstractions;

namespace Zeus.v2.Handlers.Bot.Actions.Echo
{
    public class EchoAction : IBotAction
    {
        public class Format : IBotActionFormat<EchoAction>
        {
            /// <inheritdoc />
            public bool TryParse(string message, out EchoAction action)
            {
                action = null;

                var isMatched = message.Equals("/echo", StringComparison.InvariantCultureIgnoreCase);
                if (isMatched)
                    action = new EchoAction();

                return isMatched;
            }
        }
    }
}