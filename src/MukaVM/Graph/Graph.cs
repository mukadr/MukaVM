using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MukaVM.IR.Graph
{
    public class Function
    {
        public string Name { get; set; }

        public List<BasicBlock> BasicBlocks { get; set; } = new List<BasicBlock>();

        public Function(string name)
        {
            Name = name;
        }

        public Function(string name, List<BasicBlock> basicBlocks)
            : this(name)
        {
            BasicBlocks = basicBlocks;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("FUNCTION " + Name);
            foreach (var bb in BasicBlocks)
            {
                sb.AppendLine(bb.ToString());
            }
            sb.AppendLine("END");

            return sb.ToString();
        }
    }

    public class Label : IR.Label
    {
        public BasicBlock BasicBlock { get; set; }

        public Label(BasicBlock basicBlock)
            : base(basicBlock.Name)
        {
            BasicBlock = basicBlock;
        }
    }

    public class BasicBlock
    {
        private static int _number = 1;

        public string Name { get; set; }

        public List<Instruction> Instructions { get; set; } = new List<Instruction>();

        public BasicBlock()
        {
            Name = "BB" + _number++;
        }

        public BasicBlock(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Name);
            foreach (var instruction in Instructions)
            {
                sb.AppendLine("  " + instruction.ToString());
            }

            return sb.ToString();
        }
    }

    public static class Convert
    {
        public static Function ToControlFlowGraph(IR.Function function)
        {
            var graph = new Function(function.Name);

            var bb = new BasicBlock();
            foreach (var instruction in function.Instructions)
            {
                if (instruction is IR.Label label)
                {
                    if (bb.Instructions.Count > 0)
                    {
                        graph.BasicBlocks.Add(bb);
                        bb = new BasicBlock();
                    }

                    // Update old labels to point to current basic block
                    var newLabel = new Label(bb);
                    foreach (var fi in function.Instructions)
                    {
                        if (fi is Jmp jmp && jmp.Target == label)
                        {
                            jmp.Target = newLabel;
                        }
                    }
                }
                else
                {
                    bb.Instructions.Add(instruction);

                    if (instruction is Jmp || instruction is Ret)
                    {
                        graph.BasicBlocks.Add(bb);
                        bb = new BasicBlock();
                    }
                }
            }

            if (bb.Instructions.Count > 0)
            {
                graph.BasicBlocks.Add(bb);
            }

            return graph;
        }
    }
}
