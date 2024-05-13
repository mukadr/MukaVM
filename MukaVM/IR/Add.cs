namespace MukaVM.IR;

public class Add(Var target, Value value1, Value value2) : InstructionWithTarget(target, value1, value2)
{
    public override string ToString() => Target + " = " + Operands[0] + " + " + Operands[1];
}
