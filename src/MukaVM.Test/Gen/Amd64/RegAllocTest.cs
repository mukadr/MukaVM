using Xunit;

namespace MukaVM.Test.IR
{
    public class RegAllocTest
    {
        [Fact]
        public void Single_Assignment_Allocates_Single_Register()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Multiple_Variables_Allocates_Multiple_Registers()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    y = x + 1
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        RBX = RAX + 1
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Spills_Register_To_Memory_When_Exhausted()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    y = 2
                    z = 3
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        RBX = 2
                        MEM[0] = RAX
                        RAX = 3
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Spills_Register_To_Memory_When_Exhausted_Two()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    y = x + 1
                    z = 3 + y
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        RBX = RAX + 1
                        MEM[0] = RAX
                        RAX = 3 + RBX
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Restores_Register_From_Memory_When_Needed()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    y = 2
                    z = 3
                    a = x
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        RBX = 2
                        MEM[0] = RAX
                        RAX = 3
                        MEM[1] = RAX
                        RAX = MEM[0]
                        RAX = RAX
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Restores_Register_From_Memory_When_Needed_Two()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    y = x + 3
                    z = x + 1
                    a = 5
                    b = x
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        RBX = RAX + 3
                        MEM[0] = RAX
                        RAX = RAX + 1
                        MEM[1] = RAX
                        RAX = 5
                        MEM[2] = RAX
                        RAX = MEM[0]
                        RAX = RAX
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Allocates_Between_Mulitple_Basic_Blocks()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    again
                    x = x + 1
                    IF x > 10: exit
                    JMP again
                    exit
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        <BB2>
                    }
                    BB2 {
                        <BB1, BB3>
                        RBX = RAX
                        MEM[0] = RAX
                        RAX = RBX + 1
                        IF RAX > 10: BB4
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

            Util.AssertAmd64Equals(expected, sourceText);
        }

        [Fact]
        public void Converts_PHI_Into_Mov()
        {
            const string sourceText = @"
                FUNCTION f {
                    x = 1
                    IF x > 10: yeah
                    x = 2
                    yeah
                    x = 3
                    RET
                }";

            const string expected = @"
                FUNCTION f {
                    BB1 {
                        RAX = 1
                        IF RAX > 10: BB3
                        <BB2, BB3>
                    }
                    BB2 {
                        <BB1>
                        RBX = 2
                        <BB3>
                    }
                    BB3 {
                        <BB1, BB2>
                        MEM[0] = RAX
                        RAX = 3
                        RET
                    }
                }";

            Util.AssertAmd64Equals(expected, sourceText);
        }
    }
}