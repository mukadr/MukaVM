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
                    if (instruction is InstructionWithOperands io)
                    {
                        for (var i = 0; i < io.Operands.Length; i++)
                        {
                            if (io.Operands[i] is Var v)
                            {
                                io.Operands[i] = FindSSAVariable(bb, v.Name, ref ssaVarCount);
                            }
                        }

                        if (instruction is InstructionWithTarget it)
                        {
                            it.Target = CreateSSAVariable(bb, it.Target.Name, ref ssaVarCount);
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
            
            var phiOperands = FindPhiOperands(bb, name, ref ssaVarCount);

            // Avoid redundant PHI
            var firstOperand = phiOperands.First();
            if (phiOperands.All(p => p == firstOperand))
            {
                return firstOperand;
            }

            var phiTarget = CreateSSAVariable(bb, name, ref ssaVarCount);
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