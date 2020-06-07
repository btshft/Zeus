using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zeus.v2.Handlers.Bot.Abstractions;

namespace Zeus.v2.Handlers.Bot.Actions
{
    public class BotActionParser
    {
        private readonly ICollection<Func<string, CancellationToken, Task<bool>>> _innerParsers;

        private BotActionParser(ICollection<Func<string, CancellationToken, Task<bool>>> innerParsers)
        {
            _innerParsers = innerParsers;
        }

        public static ParserBuilder Builder()
        {
            return new ParserBuilder();
        }

        public async Task<bool> ParseAsync(string message, CancellationToken cancellation = default)
        {
            foreach (var parser in _innerParsers)
            {
                if (await parser(message, cancellation))
                    return true;
            }

            return false;
        }

        public class ParserBuilder
        {
            private static readonly ConcurrentDictionary<Type, object> FormatsCache
                = new ConcurrentDictionary<Type, object>();

            private readonly ConcurrentDictionary<Type, Func<string, CancellationToken, Task<bool>>> _parsers;

            internal ParserBuilder()
            {
                _parsers = new ConcurrentDictionary<Type, Func<string, CancellationToken, Task<bool>>>();
            }

            public ParserBuilder WhenParsed<TFormat, TAction>(Func<TAction, CancellationToken, Task> continuation)
                where TFormat : IBotActionFormat<TAction>, new()
                where TAction : IBotAction
            {
                _parsers.TryAdd(typeof (TFormat), async (message, cancellation) =>
                {
                    var format = (TFormat)FormatsCache.GetOrAdd(typeof(TFormat), _ => new TFormat());
                    if (format.TryParse(message, out var action))
                    {
                        await continuation(action, cancellation);
                        return true;
                    }

                    return false;
                });

                return this;
            }

            public BotActionParser Create()
            {
                return new BotActionParser(_parsers.Values);
            }
        }
    }
}