using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Util;

namespace LibApiBrowser.Search.Lucene
{
    public class DottedAnalyzer : Analyzer
    {
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            if (fieldName == "version" || fieldName == "type")
            {
                return new TokenStreamComponents(new KeywordTokenizer(reader));
            }

            return new TokenStreamComponents(new DotTokeninzer(LuceneVersion.LUCENE_48, reader));
        }

        private class DotTokeninzer : CharTokenizer
        {
            public DotTokeninzer(LuceneVersion matchVersion, TextReader input) : base(matchVersion, input)
            {
            }

            public DotTokeninzer(LuceneVersion matchVersion, AttributeSource.AttributeFactory factory, TextReader input) : base(matchVersion, factory, input)
            {
            }

            protected override bool IsTokenChar(int c)
            {
                return char.IsLetterOrDigit((char)c);
            }
        }
    }
}