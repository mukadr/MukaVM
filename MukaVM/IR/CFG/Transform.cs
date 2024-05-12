using MukaVM.IR.Instructions;
using System.Linq;

namespace MukaVM.IR.CFG;

public static class Transform
{
    public static CfgFunction ToControlFlowGraph(Function function)
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