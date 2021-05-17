using System.Collections.Generic;
using MukaVM.IR;
using Xunit;

namespace MukaVM.Test.Graph
{
    public class CfgTest
    {
        [Fact]
        public void EmptyFunction_Generates_EmptyCfg()
        {
            var function = new Function("f", new List<Instruction>());

            const string expected = @"
                FUNCTION f {
                }";

            Util.AssertFunctionToCfgEquals(expected, function);
        }

        [Fact]
        public void SimpleFunction_Generates_One_BasicBlock()
        {
            var x = new Var("x");
            var y = new Var("y");
            var function = new Function(
                "f",
                new List<Instruction>
                {
                    new Add(x, new Int(0), new Int(10)),
                    new Add(y, x, x),
                    new Ret()
                });

            const string expected = @"
                FUNCTION f {
                  BB1 {
                    x = 0 + 10
                    y = x + x
                    RET
                  }
                }";

            Util.AssertFunctionToCfgEquals(expected, function);
        }

        [Fact]
        public void FunctionWithLabels_Generates_MoreThanOne_BasicBlock()
        {
            var x = new Var("x");
            var add5 = new Label("add5");
            var exit = new Label("exit");
            var function = new Function(
                "f",
                new List<Instruction>
                {
                    new Add(x, new Int(0), new Int(1)),
                    new Jg(x, new Int(0), add5),
                    new Add(x, x, new Int(2)),
                    new Jmp(exit),
                    add5,
                    new Add(x, x, new Int(5)),
                    exit,
                    new Ret()
                });

            const string expected = @"
                FUNCTION f {
                  BB1 {
                    x = 0 + 1
                    IF x > 0: BB3
                  }
                  BB2 {
                    x = x + 2
                    JMP BB4
                  }
                  BB3 {
                    x = x + 5
                  }
                  BB4 {
                    RET
                  }
                }";

            Util.AssertFunctionToCfgEquals(expected, function);
        }
    }
}