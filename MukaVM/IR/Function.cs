using MukaVM.IR.Instructions;
using System.Collections.Generic;
using System.Text;

namespace MukaVM.IR;

public class Function(string name, List<Instruction> instructions)
{
    public string Name { get; set; } = name;

    public List<Instruction> Instructions { get; set; } = instructions;

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