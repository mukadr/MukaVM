using System.Collections.Generic;
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("FUNCTION " + Name + " {");
            foreach (var bb in BasicBlocks)
            {
                sb.AppendLine(bb.ToString());
            }
            sb.AppendLine("}");

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
        public string Name { get; set; }

        public List<Instruction> Instructions { get; set; } = new List<Instruction>();

        public BasicBlock(int number)
        {
            Name = "BB" + number;
        }

        public BasicBlock(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("  " + Name + " {");
            foreach (var instruction in Instructions)
            {
                sb.AppendLine("    " + instruction.ToString());
            }
            sb.AppendLine("  }");

            return sb.ToString();
        }
    }

    public static class Convert
    {
        public static Function ToControlFlowGraph(IR.Function function)
        {
            var graph = new Function(function.Name);

            var n = 1;
            var bb = new BasicBlock(n);
            foreach (var instruction in function.Instructions)
            {
                if (instruction is IR.Label label)
                {
                    if (bb.Instructions.Count > 0)
                    {
                        graph.BasicBlocks.Add(bb);
                        bb = new BasicBlock(++n);
                    }

                    var startOfBB = new Label(bb);
                    foreach (var fi in function.Instructions)
                    {
                        if (fi is Jmp jmp && jmp.Target == label)
                        {
                            jmp.Target = startOfBB;
                        }
                    }
                }
                else
                {
                    bb.Instructions.Add(instruction);

                    if (instruction is Jmp || instruction is Ret)
                    {
                        graph.BasicBlocks.Add(bb);
                        bb = new BasicBlock(++n);
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
