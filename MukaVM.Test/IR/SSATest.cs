using Xunit;

namespace MukaVM.Test.IR;

public class SSATest
{
    [Fact]
    public void Single_BasicBlock_Generates_SSAVariables()
    {
        const string sourceText = @"
            FUNCTION f {
                x = 10
                y = x + 1
                RET
            }";

        const string expected = @"
            FUNCTION f {
                BB1 {
                    v1 = 10
                    v2 = v1 + 1
                    RET
                }
            }";

        Util.AssertSSAEquals(expected, sourceText);
    }

    [Fact]
    public void Multiple_BasicBlocks_Generates_Phi_Instructions()
    {
        const string sourceText = @"
            FUNCTION f {
                x = 10
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
                    v1 = 10
                    IF v1 > 5: BB3
                }
                BB2 {
                    v2 = v1 + 1
                    JMP BB4
                }
                BB3 {
                    v3 = v1 + 3
                }
                BB4 {
                    v4 = PHI(v2, v3)
                    IF v4 > 10: BB6
                }
                BB5 {
                    v5 = v4 + 5
                    JMP BB7
                }
                BB6 {
                    v6 = v4 + 1
                }
                BB7 {
                    v7 = PHI(v5, v6)
                    v8 = v7 + v4
                    RET
                }
            }";

        Util.AssertSSAEquals(expected, sourceText);
    }

    [Fact]
    public void Simple_Loop_Is_Handled_Correctly()
    {
        const string sourceText = @"
            FUNCTION f {
                x = 1
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
                    v1 = 1
                }
                BB2 {
                    v2 = PHI(v1, v3)
                    v3 = v2 + 1
                    IF v3 > 10: BB4
                }
                BB3 {
                    JMP BB2
                }
                BB4 {
                    RET
                }
            }";

        Util.AssertSSAEquals(expected, sourceText);
    }

    [Fact]
    public void Loop_With_Two_Exits_Generates_PHI()
    {
        const string sourceText = @"
            FUNCTION f {
                x = 1
                again
                x = x + 1
                IF x > 5: end1
                IF x > 10: end2
                JMP again
                end1
                x = 10 + x
                end2
                y = x + 5
                RET
            }";

        const string expected = @"
            FUNCTION f {
                BB1 {
                    v1 = 1
                }
                BB2 {
                    v2 = PHI(v1, v3)
                    v3 = v2 + 1
                    IF v3 > 5: BB5
                }
                BB3 {
                    IF v3 > 10: BB6
                }
                BB4 {
                    JMP BB2
                }
                BB5 {
                    v4 = 10 + v3
                }
                BB6 {
                    v5 = PHI(v3, v4)
                    v6 = v5 + 5
                    RET
                }
            }";

        Util.AssertSSAEquals(expected, sourceText);
    }

    [Fact]
    public void Multiple_Reaching_BasicBlocks_Generates_PHI()
    {
        const string sourceText = @"
            FUNCTION hello {
                x = 10
                y = 1
                again
                IF x > y: gt
                x = x + 1
                IF y > x: done
                JMP again
                gt
                y = y + 3
                JMP again
                done
                RET
            }";

        const string expected = @"
            FUNCTION hello {
                BB1 {
                    v1 = 10
                    v2 = 1
                }
                BB2 {
                    v3 = PHI(v1, v5)
                    v4 = PHI(v2, v6)
                    IF v3 > v4: BB5
                }
                BB3 {
                    v5 = v3 + 1
                    IF v4 > v5: BB6
                }
                BB4 {
                    JMP BB2
                }
                BB5 {
                    v6 = v4 + 3
                    JMP BB2
                }
                BB6 {
                    RET
                }
            }";

        Util.AssertSSAEquals(expected, sourceText);
    }

    [Fact]
    public void Test_With_Multiple_Variables()
    {
        const string sourceText = @"
            FUNCTION f {
                x = 1
                y = 2
                a = 0
                again
                IF x > 9: exit
                a = a + y
                x = x + 1
                JMP again
                exit
                z = a + y
                RET
            }";

        const string expected = @"
            FUNCTION f {
                BB1 {
                    v1 = 1
                    v2 = 2
                    v3 = 0
                }
                BB2 {
                    v4 = PHI(v1, v8)
                    v5 = PHI(v3, v7)
                    IF v4 > 9: BB4
                }
                BB3 {
                    v7 = v5 + v2
                    v8 = v4 + 1
                    JMP BB2
                }
                BB4 {
                    v9 = v5 + v2
                    RET
                }
            }";

        Util.AssertSSAEquals(expected, sourceText);
    }
}