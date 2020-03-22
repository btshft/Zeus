using Scriban.Parsing;
using Scriban.Runtime;

namespace Zeus.Templating.Scriban
{
    public class ScribanOptions
    {
        public ParserOptions? Parser { get; set; }

        public LexerOptions? Lexer { get; set; }

        public MemberRenamerDelegate MemberRenamer { get; set; }

        public MemberFilterDelegate MemberFilter { get; set; }

        public bool RenameMembers { get; set; }

        internal MemberRenamerDelegate GetRenamer()
        {
            return RenameMembers && MemberRenamer == null
                ? (m) => m.Name
                : MemberRenamer;
        }
    }
}