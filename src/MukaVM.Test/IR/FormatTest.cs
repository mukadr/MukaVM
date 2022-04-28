using System.Text;
using MukaVM.IR;
using Xunit;

namespace MukaVM.Test.IR;

public class FormatTest
{
    [Fact]
    public void FormatSource_TrimsStart()
    {
        const string actual = " \t \n\n X";

        const string expected = "X";

        Assert.Equal(expected, Format.FormatSource(actual));
    }

    [Fact]
    public void FormatSource_TrimsEnd()
    {
        const string actual = "X \t \n\n ";

        const string expected = "X";

        Assert.Equal(expected, Format.FormatSource(actual));
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
        expected.Append('}');

        Assert.Equal(expected.ToString(), Format.FormatSource(actual));
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
        expected.Append('}');

        Assert.Equal(expected.ToString(), Format.FormatSource(actual));
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
        expected.Append('}');

        Assert.Equal(expected.ToString(), Format.FormatSource(actual));
    }
}