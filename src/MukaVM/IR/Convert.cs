
namespace MukaVM.IR
{
    public static class Convert
    {
        public static CfgFunction ToControlFlowGraph(Function function)
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
    }
}