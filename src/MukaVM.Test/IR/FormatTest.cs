using System.Text;
using MukaVM.IR;
using Xunit;
using Xunit.Sdk;

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

        const string expected = @"FUNCTION f {
    BB1 {
        A B
        C D
    }
}";

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

        const string expected = @"FUNCTION f {
    BB1 {
        A B
        C D
    }
}";

        Assert.Equal(expected.ToString(), Format.FormatSource(actual));
    }

    [Fact]
    public void FormatSource_KeepSpacesInStrings()
    {
        const string actual = @"
            BB1 {
                WRITE ""  Hello  World  ""
                RET
            }";

        const string expected = @"BB1 {
    WRITE ""  Hello  World  ""
    RET
}";

        Assert.Equal(expected.ToString(), Format.FormatSource(actual));
    }
}