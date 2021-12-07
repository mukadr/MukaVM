using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR
{
    public static partial class Convert
    {
        public static void ToSSA(CfgFunction function)
        {
            var ssaVarCount = 1;
            foreach (var bb in function.BasicBlocks)
            {
                foreach (var instruction in bb.Instructions)
                {
                    if (instruction is Add add)
                    {
                        if (add.Value1 is Var v1)
                        {
                            add.Value1 = FindSSAVariable(bb, v1.Name, ref ssaVarCount);
                        }
                        if (add.Value2 is Var v2)
                        {
                            add.Value2 = FindSSAVariable(bb, v2.Name, ref ssaVarCount);
                        }
                        add.Target = CreateSSAVariable(bb, add.Target.Name, ref ssaVarCount);
                    }
                    else if (instruction is Jg jg)
                    {
                        if (jg.Value1 is Var v1)
                        {
                            jg.Value1 = FindSSAVariable(bb, v1.Name, ref ssaVarCount);
                        }
                        if (jg.Value2 is Var v2)
                        {
                            jg.Value2 = FindSSAVariable(bb, v2.Name, ref ssaVarCount);
                        }
                    }
                }
            }
        }

        private static SSAVar CreateSSAVariable(BasicBlock bb, string name, ref int ssaVarCount)
        {
            var ssaVar = new SSAVar(ssaVarCount++);
            bb.SSAVariables.Add(name, ssaVar);
            return ssaVar;
        }

        private static SSAVar FindSSAVariable(BasicBlock bb, string name, ref int ssaVarCount)
        {
            if (bb.SSAVariables.TryGetValue(name, out var ssaVar))
            {
                return ssaVar;
            }

            return FindSSAVariableRecursive(bb, name, ref ssaVarCount);
        }

        private static SSAVar FindSSAVariableRecursive(BasicBlock bb, string name, ref int ssaVarCount)
        {
            if (bb.ReachedBy.Count == 1)
            {
                return FindSSAVariable(bb.ReachedBy.Single().Value, name, ref ssaVarCount);
            }
            
            var phiTarget = CreateSSAVariable(bb, name, ref ssaVarCount);
            var phiOperands = FindPhiOperands(bb, name, ref ssaVarCount);
            bb.Phis.Add(new Phi(phiTarget, phiOperands));
            return phiTarget;
        }

        private static List<SSAVar> FindPhiOperands(BasicBlock currentBB, string name, ref int ssaVarCount)
        {
            var operands = new List<SSAVar>();
            foreach (var bb in currentBB.ReachedBy)
            {
                operands.Add(FindSSAVariable(bb.Value, name, ref ssaVarCount));
            }
            return operands;
        }
    }
}