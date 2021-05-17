using System.Collections.Generic;
using System.Text;

namespace MukaVM.IR
{
    public class CfgFunction
    {
        public string Name { get; set; }

        public List<BasicBlock> BasicBlocks { get; set; } = new List<BasicBlock>();

        public CfgFunction(string name)
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

    public class CfgLabel : IR.Label
    {
        public BasicBlock BasicBlock { get; set; }

        public CfgLabel(BasicBlock basicBlock)
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
}
