using MukaVM.IR.Instructions;
using System.Collections.Generic;

namespace MukaVM.IR.SSA;

public class Phi : Instruction
{
    public SSAVar Target { get; set; }

    public List<SSAVar> Operands { get; set; }

    public Phi(SSAVar target, List<SSAVar> operands)
    {
        Target = target;
        Operands = operands;
    }

    public override string ToString() => Target + " = PHI(" + string.Join(", ", Operands) + ")";
}
