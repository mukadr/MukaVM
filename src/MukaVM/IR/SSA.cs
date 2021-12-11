using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR
{
    public class SSA
    {
        private int _variableNumber = 1;

        private SSA() { }

        public static void Transform(CfgFunction function)
        {
            new SSA().TransformFunction(function);
        }

        public void TransformFunction(CfgFunction function)
        {
            function.BasicBlocks.ForEach(ConvertToSSA);
            function.BasicBlocks.ForEach(UpdatePhiOperands);
        }

        private void ConvertToSSA(BasicBlock bb)
        {
            bb.Instructions.ForEach(i => UpdateSSAVariablesForInstruction(bb, i));
        }

        private void UpdateSSAVariablesForInstruction(BasicBlock bb, Instruction instruction)
        {
            if (instruction is InstructionWithOperands io)
            {
                UpdateSSAVariablesForInstructionOperands(bb, io);

                if (io is InstructionWithTarget it)
                {
                    CreateSSAVariableForInstructionTarget(bb, it);
                }
            }
        }

        private void UpdateSSAVariablesForInstructionOperands(BasicBlock bb, InstructionWithOperands io)
        {
            for (var i = 0; i < io.Operands.Length; i++)
            {
                if (io.Operands[i] is Var var)
                {
                    io.Operands[i] = FindOrCreateSSAVariable(bb, var);
                }
            }
        }

        private void CreateSSAVariableForInstructionTarget(BasicBlock bb, InstructionWithTarget it)
        {
            it.Target = InsertSSAVariable(bb, it.Target);
        }

        private SSAVar FindOrCreateSSAVariable(BasicBlock bb, Var var)
        {
            if (bb.SSAVariables.TryGetValue(var.Name, out var ssaVar))
            {
                return ssaVar;
            }

            return CreateSSAVariable(bb, var);
        }

        private SSAVar CreateSSAVariable(BasicBlock bb, Var var)
        {
            var phiTarget = InsertSSAVariable(bb, var);

            var phiOperands = LookupPhiOperands(bb, var);

            // Avoid PHI with only one operand
            var firstOperand = phiOperands.First();
            if (phiOperands.All(p => p == firstOperand))
            {
                // Remove unused SSA variable
                bb.SSAVariables.Remove(var.Name);

                // Decrement counter to keep tests sane
                _variableNumber--;

                return firstOperand;
            }

            bb.Phis.Add(new Phi(phiTarget, phiOperands));

            return phiTarget;
        }

        private SSAVar InsertSSAVariable(BasicBlock bb, Var var)
        {
            var ssaVar = new SSAVar(_variableNumber++, var);
            bb.SSAVariables[var.Name] = ssaVar;
            return ssaVar;
        }

        private List<SSAVar> LookupPhiOperands(BasicBlock currentBB, Var var)
        {
            var operands = new List<SSAVar>();
            foreach (var bb in currentBB.ReachedBy)
            {
                operands.Add(FindOrCreateSSAVariable(bb.Value, var));
            }
            return operands;
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
    }
}
