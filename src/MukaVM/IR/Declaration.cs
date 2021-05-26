using System.Collections.Generic;
using System.Text;

namespace MukaVM.IR
{
    public class Function
    {
        public string Name { get; set; }

        public List<Instruction> Instructions { get; set; }

        public Function(string name, List<Instruction> instructions)
        {
            Name = name;
            Instructions = instructions;
        }

        public Function(string name)
            : this(name, new List<Instruction>())
        { }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("FUNCTION " + Name + " {");
            foreach (var instruction in Instructions)
            {
                sb.AppendLine(Format.Indent(1) + instruction.ToString());
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
