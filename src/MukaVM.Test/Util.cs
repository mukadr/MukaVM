using MukaVM.IR;
using System.Text;
using Xunit;

namespace MukaVM.Test
{
    internal static class Util
    {
        internal static void AssertControlFlowGraphEquals(string expected, Function function)
        {
            AssertSourceEquals(expected, IR.Graph.Convert.ToControlFlowGraph(function).ToString());
        }

        internal static void AssertSourceEquals(string expected, string actual)
        {
            Assert.Equal(ReindentSource(expected), ReindentSource(actual));
        }

        internal static string ReindentSource(string source)
        {
            var sb = new StringBuilder();

            var atStartOfSource = true;
            var atStartOfLine = false;
            var foundSpaces = false;
            var indent = 0;
            var insideString = false;

            for (var i = 0; i < source.Length; i++)
            {
                if (atStartOfSource && char.IsWhiteSpace(source[i]))
                {
                    continue;
                }

                atStartOfSource = false;

                if (insideString)
                {
                    sb.Append(source[i]);
                    if (source[i] == '"')
                    {
                        insideString = false;
                    }
                    continue;
                }

                if (source[i] == ' ')
                {
                    foundSpaces = true;
                    continue;
                }

                if (source[i] == '\n' || source[i] == '\r')
                {
                    atStartOfLine = true;
                    foundSpaces = false;
                    continue;
                }

                if (!char.IsWhiteSpace(source[i]))
                {
                    var shouldIndent = false;

                    if (atStartOfLine)
                    {
                        sb.AppendLine();
                        atStartOfLine = false;
                        shouldIndent = true;
                    }

                    if (source[i] == '}')
                    {
                        indent--;
                    }

                    if (foundSpaces)
                    {
                        if (shouldIndent)
                        {
                            for (var j = 0; j < indent; j++)
                            {
                                sb.Append("  ");
                            }
                        }
                        else
                        {
                            sb.Append(" ");
                        }

                        foundSpaces = false;
                    }

                    if (source[i] == '{')
                    {
                        indent++;
                    }

                    sb.Append(source[i]);

                    if (source[i] == '"')
                    {
                        insideString = true;
                    }
                }
            }

            return sb.ToString();
        }
    }

    public class UtilTest
    {
        [Fact]
        public void ReindentSource_TrimsStart()
        {
            const string expected = "X";
            const string actual = " \t \n\n X";

            Assert.Equal(expected, Util.ReindentSource(actual));
        }

        [Fact]
        public void ReindentSource_TrimsEnd()
        {
            const string expected = "X";
            const string actual = "X \t \n\n ";

            Assert.Equal(expected, Util.ReindentSource(actual));
        }

        [Fact]
        public void ReindentSource_FixesIndentation()
        {
            const string expected = @"BB1 {
  A B
  C D
}";
            const string actual = @"
                BB1 {
                  A B
                  C D
                }";

            Assert.Equal(expected, Util.ReindentSource(actual));
        }

        [Fact]
        public void ReindentSource_RespectStrings()
        {
            const string expected = @"BB1 {
  WRITE " + "\"  Hello  World  \"" + @"
  RET
}";
            const string actual = @"
                BB1 {
                  WRITE " + "\"  Hello  World  \"" + @"
                  RET
                }";

            Assert.Equal(expected, Util.ReindentSource(actual));
        }
    }
}
