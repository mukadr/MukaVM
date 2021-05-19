using System.Text;

namespace MukaVM.IR
{
    public static class Format
    {
        public const string Indentation = "    ";

        public static string Indent(int count)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                sb.Append(Indentation);
            }

            return sb.ToString();
        }

        public static string FormatSource(string source)
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
                        sb.Append(IR.Format.Indent(indentLevel));
                    }
                    else if (foundSpaces)
                    {
                        sb.Append(' ');
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
}
