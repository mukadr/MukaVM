using MukaVM.IR;
using Xunit;

namespace MukaVM.Test.IR
{
    public class ControlFlowGraphTest
    {
        [Fact]
        public void EmptyFunction_Generates_EmptyCfg()
        {
            const string sourceText = @"
                FUNCTION f {
                }";

            const string expected = @"
                FUNCTION f {
                }";

            Util.AssertControlFlowGraphEquals(expected, Parse.FromSourceText(sourceText));
        }

        [Fact]
        public void SimpleFunction_Generates_One_BasicBlock()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 0 + 10
                    y = x + x
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        x = 0 + 10
                        y = x + x
                        RET
                    }
                }";

            Util.AssertControlFlowGraphEquals(expected, Parse.FromSourceText(sourceText));
        }

        [Fact]
        public void FunctionWithLabels_Generates_MoreThanOne_BasicBlock()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 0 + 1
                    IF x > 0: add5
                    x = x + 2
                    JMP exit
                    add5
                    x = x + 5
                    exit
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        x = 0 + 1
                        IF x > 0: BB3
                        <BB2, BB3>
                    }
                    BB2 {
                        <BB1>
                        x = x + 2
                        JMP BB4
                        <BB4>
                    }
                    BB3 {
                        <BB1>
                        x = x + 5
                        <BB4>
                    }
                    BB4 {
                        <BB2, BB3>
                        RET
                    }
                }";

            Util.AssertControlFlowGraphEquals(expected, Parse.FromSourceText(sourceText));
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
                        x = 0 + 1
                        <BB2>
                    }
                    BB2 {
                        <BB1, BB3>
                        x = x + 1
                        IF x > 10: BB4
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

            Util.AssertControlFlowGraphEquals(expected, Parse.FromSourceText(sourceText));
        }
    }
}