using System.Collections.Generic;

namespace MukaVM.IR.SSA;

public class Phi(SSAVar target, List<SSAVar> operands) : Instruction
{
    public SSAVar Target { get; set; } = target;

    public List<SSAVar> Operands { get; set; } = operands;

    public override string ToString() => Target + " = PHI(" + string.Join(", ", Operands) + ")";
}
