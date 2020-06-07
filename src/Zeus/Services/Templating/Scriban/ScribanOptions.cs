using Scriban.Parsing;
using Scriban.Runtime;

namespace Zeus.v2.Services.Templating.Scriban
{
    public class ScribanOptions
    {
        public ParserOptions? Parser { get; set; }

        public LexerOptions? Lexer { get; set; }

        public MemberRenamerDelegate MemberRenamer { get; set; } = (m) => m.Name;

        public MemberFilterDelegate MemberFilter { get; set; }
    }
}