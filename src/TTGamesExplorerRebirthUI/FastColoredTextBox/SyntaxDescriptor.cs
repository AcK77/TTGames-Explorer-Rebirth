using System.Text.RegularExpressions;

namespace FastColoredTextBoxNS
{
    public class SyntaxDescriptor : IDisposable
    {
        private bool _disposedValue;

        public char LeftBracket = '(';
        public char RightBracket = ')';
        public char LeftBracket2 = '{';
        public char RightBracket2 = '}';
        public BracketsHighlightStrategy BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
        public readonly List<Style> Styles = [];
        public readonly List<RuleDesc> Rules = [];
        public readonly List<FoldingDesc> Foldings = [];

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var style in Styles)
                    {
                        style.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class RuleDesc
    {
        private Regex _regex;

        public string Pattern;
        public RegexOptions Options = RegexOptions.None;
        public Style Style;

        public Regex Regex
        {
            get
            {
                _regex ??= new Regex(Pattern, SyntaxHighlighter.RegexCompiledOption | Options);

                return _regex;
            }
        }
    }

    public class FoldingDesc
    {
        public string StartMarkerRegex;
        public string FinishMarkerRegex;
        public RegexOptions Options = RegexOptions.None;
    }
}