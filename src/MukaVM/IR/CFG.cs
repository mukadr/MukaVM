using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MukaVM.IR;

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

public class CfgLabel : Label
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

public static class CFG
{
    public static CfgFunction Convert(Function function)
    {
        var cfg = BuildControlFlowGraph(function);

        ConnectControlFlowGraph(cfg);

        return cfg;
    }

    private static CfgFunction BuildControlFlowGraph(Function function)
    {
        var cfg = new CfgFunction(function.Name);

        var n = 1;
        var bb = new BasicBlock(n);
        foreach (var instruction in function.Instructions)
        {
            if (instruction is Label label)
            {
                if (bb.Instructions.Count > 0)
                {
                    cfg.BasicBlocks.Add(bb);
                    bb = new BasicBlock(++n);
                }

                var startOfBB = new CfgLabel(bb);
                foreach (var ins in function.Instructions)
                {
                    if (ins is Jmp jmp && jmp.Target == label)
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
                    cfg.BasicBlocks.Add(bb);
                    bb = new BasicBlock(++n);
                }
            }
        }

        if (bb.Instructions.Count > 0)
        {
            cfg.BasicBlocks.Add(bb);
        }

        return cfg;
    }

    private static void ConnectControlFlowGraph(CfgFunction cfg)
    {
        BasicBlock? previous = null;

        foreach (var bb in cfg.BasicBlocks)
        {
            if (previous is not null)
            {
                previous.FollowedBy.Add(bb.Name, bb);
                bb.ReachedBy.Add(previous.Name, previous);
                previous = null;
            }

            if (bb.Instructions.Last() is Jmp jmp)
            {
                var target = ((CfgLabel)jmp.Target).BasicBlock;

                bb.FollowedBy.Add(target.Name, target);
                target.ReachedBy.Add(bb.Name, bb);

                if (jmp.GetType() != typeof(Jmp))
                {
                    // Conditional JMP follows next BB
                    previous = bb;
                }
            }
            else
            {
                previous = bb;
            }
        }
    }
}