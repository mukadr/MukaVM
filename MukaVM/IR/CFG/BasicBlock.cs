using MukaVM.IR.Instructions;
using MukaVM.IR.SSA;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MukaVM.IR.CFG;

public class BasicBlock
{
    public string Name { get; set; }

    public List<Phi> Phis { get; set; } = new();

    public List<Instruction> Instructions { get; set; } = new();

    public SortedList<string, BasicBlock> ReachedBy { get; set; } = new();

    public SortedList<string, BasicBlock> FollowedBy { get; set; } = new();

    public Dictionary<string, SSAVar> SSAVariables { get; set; } = new();

    public BasicBlock(int number)
    {
        Name = "BB" + number;
    }

    public BasicBlock(string name)
    {
        Name = name;
    }

    public override string ToString() => ConvertToString();

    public string ConvertToString(bool emitReachedByAndFollowedBy = false)
    {
        var sb = new StringBuilder();

        sb.AppendLine(Format.Indent(1) + Name + " {");

        if (emitReachedByAndFollowedBy && ReachedBy.Count > 0)
        {
            sb.AppendLine(Format.Indent(2) + "<" + string.Join(", ", ReachedBy.Select(kv => kv.Key)) + ">");
        }

        foreach (var phi in Phis)
        {
            sb.AppendLine(Format.Indent(2) + phi.ToString());
        }

        foreach (var instruction in Instructions)
        {
            sb.AppendLine(Format.Indent(2) + instruction.ToString());
        }

        if (emitReachedByAndFollowedBy && FollowedBy.Count > 0)
        {
            sb.AppendLine(Format.Indent(2) + "<" + string.Join(", ", FollowedBy.Select(kv => kv.Key)) + ">");
        }

        sb.AppendLine(Format.Indent(1) + "}");

        return sb.ToString();
    }
}