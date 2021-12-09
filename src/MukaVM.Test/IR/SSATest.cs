using MukaVM.IR;
using Xunit;

namespace MukaVM.Test.IR
{
    public class SSATest
    {
        [Fact]
        public void Single_BasicBlock_Generates_SSAVariables()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 0 + 10
                    y = x + 1
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        v1 = 0 + 10
                        v2 = v1 + 1
                        RET
                    }
                }";

            Util.AssertSSAEquals(expected, Parse.FromSourceText(sourceText));
        }

        [Fact]
        public void Multiple_BasicBlocks_Generates_Phi_Instructions()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 0 + 10
                    IF x > 5: gt5
                    y = x + 1
                    JMP end1
                    gt5
                    y = x + 3
                    end1
                    IF y > 10: gt10
                    z = y + 5
                    JMP end2
                    gt10
                    z = y + 1
                    end2
                    t = z + y
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        v1 = 0 + 10
                        IF v1 > 5: BB3
                        <BB2, BB3>
                    }
                    BB2 {
                        <BB1>
                        v2 = v1 + 1
                        JMP BB4
                        <BB4>
                    }
                    BB3 {
                        <BB1>
                        v3 = v1 + 3
                        <BB4>
                    }
                    BB4 {
                        <BB2, BB3>
                        v4 = PHI(v2, v3)
                        IF v4 > 10: BB6
                        <BB5, BB6>
                    }
                    BB5 {
                        <BB4>
                        v5 = v4 + 5
                        JMP BB7
                        <BB7>
                    }
                    BB6 {
                        <BB4>
                        v6 = v4 + 1
                        <BB7>
                    }
                    BB7 {
                        <BB5, BB6>
                        v7 = PHI(v5, v6)
                        v8 = v7 + v4
                        RET
                    }
                }";

            Util.AssertSSAEquals(expected, Parse.FromSourceText(sourceText));
        }

        [Fact]
        public void FunctionWithLoops_Is_Handled_Correctly()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 0 + 1
                    again
                    x = x + 1
                    IF x > 10: end
                    JMP again
                    end
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        v1 = 0 + 1
                        <BB2>
                    }
                    BB2 {
                        <BB1, BB3>
                        v2 = PHI(v1, v3)
                        v3 = v2 + 1
                        IF v3 > 10: BB4
                        <BB3, BB4>
                    }
                    BB3 {
                        <BB2>
                        JMP BB2
                        <BB2>
                    }
                    BB4 {
                        <BB2>
                        RET
                    }
                }";

            Util.AssertSSAEquals(expected, Parse.FromSourceText(sourceText));
        }
    }
}