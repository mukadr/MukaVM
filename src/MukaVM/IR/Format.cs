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
    }
}
