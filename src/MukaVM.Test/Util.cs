using MukaVM.IR;
using System.Text;
using Xunit;

namespace MukaVM.Test
{
    internal static class Util
    {
        internal static void AssertControlFlowGraphEquals(string expected, Function actual)
        {
            AssertSourceEquals(expected, Convert.ToControlFlowGraph(actual).ToString());
        }

        internal static void AssertSourceEquals(string expected, string actual)
        {
            Assert.Equal(FormatSource(expected), FormatSource(actual));
        }

        internal static string FormatSource(string source)
        {
            var sb = new StringBuilder();

            var atStartOfSource = true;
            var atStartOfLine = false;
            var foundSpaces = false;
            var indentLevel = 0;
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
                        indentLevel--;
                    }

                    if (shouldIndent)
                    {
                        sb.Append(Format.Indent(indentLevel));
                    }
                    else if (foundSpaces)
                    {
                        sb.Append(" ");
                    }

                    foundSpaces = false;

                    if (source[i] == '{')
                    {
                        indentLevel++;
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
        public void FormatSource_TrimsStart()
        {
            const string actual = " \t \n\n X";

            const string expected = "X";

            Assert.Equal(expected, Util.FormatSource(actual));
        }

        [Fact]
        public void FormatSource_TrimsEnd()
        {
            const string actual = "X \t \n\n ";

            const string expected = "X";

            Assert.Equal(expected, Util.FormatSource(actual));
        }

        [Fact]
        public void FormatSource_AddsIndentation()
        {
            const string actual = @"
FUNCTION f {
BB1 {
A B
C D
}
}";

            var expected = new StringBuilder();
            expected.AppendLine("FUNCTION f {");
            expected.AppendLine(Format.Indent(1) + "BB1 {");
            expected.AppendLine(Format.Indent(2) + "A B");
            expected.AppendLine(Format.Indent(2) + "C D");
            expected.AppendLine(Format.Indent(1) + "}");
            expected.Append("}");

            Assert.Equal(expected.ToString(), Util.FormatSource(actual));
        }

        [Fact]
        public void FormatSource_FixesIndentation()
        {
            const string actual = @"
            FUNCTION f {
      BB1 {
              A B
          C D
                      }
                             }";

            var expected = new StringBuilder();
            expected.AppendLine("FUNCTION f {");
            expected.AppendLine(Format.Indent(1) + "BB1 {");
            expected.AppendLine(Format.Indent(2) + "A B");
            expected.AppendLine(Format.Indent(2) + "C D");
            expected.AppendLine(Format.Indent(1) + "}");
            expected.Append("}");

            Assert.Equal(expected.ToString(), Util.FormatSource(actual));
        }

        [Fact]
        public void FormatSource_RespectStrings()
        {
            const string actual = @"
                BB1 {
                    WRITE " + "\"  Hello  World  \"" + @"
                    RET
                }";

            var expected = new StringBuilder();
            expected.AppendLine("BB1 {");
            expected.AppendLine(Format.Indent(1) + "WRITE \"  Hello  World  \"");
            expected.AppendLine(Format.Indent(1) + "RET");
            expected.Append("}");

            Assert.Equal(expected.ToString(), Util.FormatSource(actual));
        }
    }
}
