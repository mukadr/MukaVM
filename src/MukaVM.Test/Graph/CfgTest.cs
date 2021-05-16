using System.Collections.Generic;
using MukaVM.IR;
using Xunit;

namespace MukaVM.Test.Graph
{
    public class CfgTest
    {
        [Fact]
        public void Test1()
        {
            var x = new Var("x");
            var greaterThanZero = new Label("greaterThanZero");
            var exit = new Label("exit");

            var f1 = new Function(
                "f1",
                new List<Instruction>
                {
                    new Add(x, new Int(0), new Int(1)),
                    new Jg(x, new Int(0), greaterThanZero),
                    new Add(x, x, new Int(2)),
                    new Jmp(exit),
                    greaterThanZero,
                    new Add(x, x, new Int(5)),
                    exit,
                    new Ret()
                });

            var cfg = IR.Graph.Convert.ToControlFlowGraph(f1);

            var bb1 = new IR.Graph.BasicBlock("BB1");
            var bb2 = new IR.Graph.BasicBlock("BB2");
            var bb3 = new IR.Graph.BasicBlock("BB3");
            var bb4 = new IR.Graph.BasicBlock("BB4");
            
            var bb3Label = new IR.Graph.Label(bb3);
            var bb4Label = new IR.Graph.Label(bb4);
            
            bb1.Instructions =
                new List<Instruction>
                {
                    new Add(x, new Int(0), new Int(1)),
                    new Jg(x, new Int(0), bb3Label)
                };

            bb2.Instructions =
                new List<Instruction>
                {
                    new Add(x, x, new Int(2)),
                    new Jmp(bb4Label)
                };

            bb3.Instructions =
                new List<Instruction>
                {
                    new Add(x, x, new Int(5))
                };

            bb4.Instructions =
                new List<Instruction>
                {
                    new Ret()
                };

            var expectedCfg = new IR.Graph.Function(
                f1.Name,
                new List<IR.Graph.BasicBlock>
                {
                    bb1,
                    bb2,
                    bb3,
                    bb4
                });

            Assert.Equal(expectedCfg.ToString(), cfg.ToString());
        }
    }
}