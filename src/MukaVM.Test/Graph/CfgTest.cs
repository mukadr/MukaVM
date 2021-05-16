using System.Collections.Generic;
using MukaVM.IR;
using Xunit;

namespace MukaVM.Test.Graph
{
    public class CfgTest
    {
        [Fact]
        public void GeneratesMoreThanOneBasicBlockForFunctionWithLabels()
        {
            var x = new Var("x");
            var gt0 = new Label("gt0");
            var exit = new Label("exit");
            var simple1 = new Function(
                "simple1",
                new List<Instruction>
                {
                    new Add(x, new Int(0), new Int(1)),
                    new Jg(x, new Int(0), gt0),
                    new Add(x, x, new Int(2)),
                    new Jmp(exit),
                    gt0,
                    new Add(x, x, new Int(5)),
                    exit,
                    new Ret()
                });

            var cfg = IR.Graph.Convert.ToControlFlowGraph(simple1);

            var bb1 = new IR.Graph.BasicBlock("BB1");
            var bb2 = new IR.Graph.BasicBlock("BB2");
            var bb3 = new IR.Graph.BasicBlock("BB3");
            var bb4 = new IR.Graph.BasicBlock("BB4");
            
            var bb3Label = new IR.Graph.Label(bb3);
            var bb4Label = new IR.Graph.Label(bb4);

            // BB1
            bb1.Instructions.Add(new Add(x, new Int(0), new Int(1)));
            bb1.Instructions.Add(new Jg(x, new Int(0), bb3Label));

            // BB2
            bb2.Instructions.Add(new Add(x, x, new Int(2)));
            bb2.Instructions.Add(new Jmp(bb4Label));

            // BB3
            bb3.Instructions.Add(new Add(x, x, new Int(5)));

            // BB4
            bb4.Instructions.Add(new Ret());

            var expectedCfg = new IR.Graph.Function(
                simple1.Name,
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