using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR
{
    public static class SSA
    {
        public static void Transform(CfgFunction function)
        {
            new Transformer().Transform(function);
        }

        private class Transformer
        {
            private int _variableNumber = 1;

            public void Transform(CfgFunction function)
            {
                function.BasicBlocks.ForEach(ConvertToSSA);
                function.BasicBlocks.ForEach(UpdatePhiOperands);
            }

            private void ConvertToSSA(BasicBlock bb)
            {
                foreach (var instruction in bb.Instructions)
                {
                    if (instruction is InstructionWithOperands io)
                    {
                        for (var i = 0; i < io.Operands.Length; i++)
                        {
                            if (io.Operands[i] is Var var)
                            {
                                io.Operands[i] = FindOrCreateSSAVariable(bb, var);
                            }
                        }

                        if (instruction is InstructionWithTarget it)
                        {
                            it.Target = CreateSSAVariable(bb, it.Target);
                        }
                    }
                }
            }

            private void UpdatePhiOperands(BasicBlock bb)
            {
                foreach (var phi in bb.Phis)
                {
                    for (var i = 0; i < phi.Operands.Count; i++)
                    {
                        // If target is inside operand list, we have a loop
                        // Update operand with the latest version of this variable
                        if (phi.Operands[i] == phi.Target)
                        {
                            phi.Operands[i] = bb.SSAVariables[phi.Operands[i].Var.Name];
                        }
                    }
                }
            }

            private SSAVar CreateSSAVariable(BasicBlock bb, Var var)
            {
                var ssaVar = new SSAVar(_variableNumber++, var);
                bb.SSAVariables[var.Name] = ssaVar;
                return ssaVar;
            }

            private SSAVar FindOrCreateSSAVariable(BasicBlock bb, Var var)
            {
                // Find existing SSA variable in current basic block
                if (bb.SSAVariables.TryGetValue(var.Name, out var ssaVar))
                {
                    return ssaVar;
                }

                // Not found, create new SSA variable
                var phiTarget = CreateSSAVariable(bb, var);

                // Lookup PHI operands
                var phiOperands = FindPhiOperands(bb, var);

                // Avoid PHI with only one operand
                var firstOperand = phiOperands.First();
                if (phiOperands.All(p => p == firstOperand))
                {
                    // Remove unused PHI variable
                    bb.SSAVariables.Remove(var.Name);

                    // Decrement counter to keep the sequence easy to follow during tests
                    _variableNumber--;

                    return firstOperand;
                }

                // Add PHI instruction to basic block
                bb.Phis.Add(new Phi(phiTarget, phiOperands));

                // Return PHI variable
                return phiTarget;
            }

            private List<SSAVar> FindPhiOperands(BasicBlock currentBB, Var var)
            {
                var operands = new List<SSAVar>();
                foreach (var bb in currentBB.ReachedBy)
                {
                    operands.Add(FindOrCreateSSAVariable(bb.Value, var));
                }
                return operands;
            }
        }
    }
}
