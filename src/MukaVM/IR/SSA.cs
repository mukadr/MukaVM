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

                // Update PHI operands in the presence of loops
                foreach (var bb in function.BasicBlocks)
                {
                    foreach (var phi in bb.Phis)
                    {
                        for (var i = 0; i < phi.Operands.Count; i++)
                        {
                            // If target is inside operand list, we have a loop
                            // Update operand with the latest version of this variable
                            if (phi.Operands[i] == phi.Target)
                            {
                                phi.Operands[i] = bb.SSAVariables[phi.Operands[i].VName];
                            }
                        }
                    }
                }
            }

            private SSAVar CreateSSAVariable(BasicBlock bb, string name)
            {
                var ssaVar = new SSAVar(_varCounter++, name);
                bb.SSAVariables[name] = ssaVar;
                return ssaVar;
            }

            private SSAVar FindSSAVariable(BasicBlock bb, string name)
            {
                // Find existing SSA variable in current basic block
                if (bb.SSAVariables.TryGetValue(name, out var ssaVar))
                {
                    return ssaVar;
                }

                // Not found, create new SSA variable
                var phiTarget = CreateSSAVariable(bb, name);

                // Lookup PHI operands
                var phiOperands = FindPhiOperands(bb, name);

                // Avoid PHI with only one operand
                var firstOperand = phiOperands.First();
                if (phiOperands.All(p => p == firstOperand))
                {
                    // Remove unused PHI variable
                    bb.SSAVariables.Remove(name);

                    // Decrement counter to keep the sequence easy to follow during tests
                    _varCounter--;

                    return firstOperand;
                }

                // Add PHI instruction to basic block
                bb.Phis.Add(new Phi(phiTarget, phiOperands));

                // Return PHI variable
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
