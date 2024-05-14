using System.Linq;

namespace MukaVM.IR;

public class Print(params Value[] operands) : InstructionWithOperands(operands)
{
    public override string ToString() => "PRINT " + string.Join(", ", Operands.Select(o => o.ToString()));
}
