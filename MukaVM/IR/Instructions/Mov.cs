namespace MukaVM.IR.Instructions;

public class Mov(Var target, Value value) : InstructionWithTarget(target, value)
{
    public override string ToString() => Target + " = " + Operands[0];
}
