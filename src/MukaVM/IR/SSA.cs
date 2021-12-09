using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR
{
    public static class SSA
    {
        public static void Transform(CfgFunction function)
        {
            new SSAPass().Transform(function);
        }

        private class SSAPass
        {
            private int _varCounter = 1;

            public void Transform(CfgFunction function)
            {
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
                                    io.Operands[i] = FindSSAVariable(bb, v.Name);
                                }
                            }

                            if (instruction is InstructionWithTarget it)
                            {
                                it.Target = CreateSSAVariable(bb, it.Target.Name);
                            }
                        }
                    }
                }
            }

            private SSAVar CreateSSAVariable(BasicBlock bb, string name)
            {
                var ssaVar = new SSAVar(_varCounter++);
                bb.SSAVariables.Add(name, ssaVar);
                return ssaVar;
            }

            private SSAVar FindSSAVariable(BasicBlock bb, string name)
            {
                if (bb.SSAVariables.TryGetValue(name, out var ssaVar))
                {
                    return ssaVar;
                }

                return FindSSAVariableRecursive(bb, name);
            }

            private SSAVar FindSSAVariableRecursive(BasicBlock bb, string name)
            {
                if (bb.ReachedBy.Count == 1)
                {
                    return FindSSAVariable(bb.ReachedBy.Single().Value, name);
                }

                var phiOperands = FindPhiOperands(bb, name);

                // Avoid redundant PHI
                var firstOperand = phiOperands.First();
                if (phiOperands.All(p => p == firstOperand))
                {
                    return firstOperand;
                }

                var phiTarget = CreateSSAVariable(bb, name);
                bb.Phis.Add(new Phi(phiTarget, phiOperands));
                return phiTarget;
            }

            private List<SSAVar> FindPhiOperands(BasicBlock currentBB, string name)
            {
                var operands = new List<SSAVar>();
                foreach (var bb in currentBB.ReachedBy)
                {
                    operands.Add(FindSSAVariable(bb.Value, name));
                }
                return operands;
            }
        }
    }
}