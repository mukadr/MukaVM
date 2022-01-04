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
            // Single predecessor, no PHI needed
            if (bb.ReachedBy.Count == 1)
            {
                return FindOrCreateSSAVariable(bb.ReachedBy.Single().Value, var);
            }

            return CreatePhiForSSAVariable(bb, var);
        }

        private SSAVar CreatePhiForSSAVariable(BasicBlock bb, Var var)
        {
            var phiTarget = InsertSSAVariable(bb, var);
            var phiOperands = LookupPhiOperands(bb, var);

            var ssaVarWithoutPhi = RemoveUnneededPhi(bb, phiOperands);
            if (ssaVarWithoutPhi is not null)
            {
                return ssaVarWithoutPhi;
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

        private SSAVar? RemoveUnneededPhi(BasicBlock bb, List<SSAVar> operands)
        {
            var firstOperand = operands.First();
            if (operands.All(o => o == firstOperand))
            {
                RemoveUnneededSSAVariable(bb, firstOperand.Var.Name);
                return firstOperand;
            }
            return null;
        }

        private void RemoveUnneededSSAVariable(BasicBlock bb, string varName)
        {
            bb.SSAVariables.Remove(varName);
            _variableNumber--;
        }

        private List<SSAVar> LookupPhiOperands(BasicBlock currentBB, Var var)
        {
            return currentBB.ReachedBy
                .Select(bb => FindOrCreateSSAVariable(bb.Value, var))
                .ToList();
        }

        private void UpdatePhiOperands(BasicBlock bb)
        {
            foreach (var phi in bb.Phis)
            {
                for (var i = 0; i < phi.Operands.Count; i++)
                {
                    // If target is inside operand list, we have a loop
                    // Update operand with the latest SSA variable
                    if (phi.Operands[i] == phi.Target)
                    {
                        phi.Operands[i] = FindLatestSSAVar(bb, bb, phi.Operands[i]);
                    }
                }
            }
        }

        private SSAVar FindLatestSSAVar(BasicBlock finalBB, BasicBlock searchBB, SSAVar current)
        {
            foreach (var bb in searchBB.ReachedBy)
            {
                if (bb.Value.SSAVariables.TryGetValue(current.Var.Name, out var v) && v.N > current.N)
                {
                    current = v;
                }

                if (bb.Value == finalBB)
                {
                    continue;
                }

                current = FindLatestSSAVar(finalBB, bb.Value, current);
            }

            return current;
        }
    }
}
