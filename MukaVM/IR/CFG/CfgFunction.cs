using System.Collections.Generic;
using System.Text;

namespace MukaVM.IR.CFG;

public class CfgFunction
{
    public string Name { get; set; }

    public List<BasicBlock> BasicBlocks { get; set; } = new();

    public CfgFunction(string name)
    {
        Name = name;
    }

    public override string ToString() => ConvertToString();

    public string ConvertToString(bool emitReachedByAndFollowedBy = false)
    {
        var sb = new StringBuilder();

        sb.AppendLine("FUNCTION " + Name + " {");

        foreach (var bb in BasicBlocks)
        {
            sb.AppendLine(bb.ConvertToString(emitReachedByAndFollowedBy));
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}