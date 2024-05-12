namespace MukaVM.IR.Instructions;

public class Add : InstructionWithTarget
{
    public Add(Var target, Value value1, Value value2)
        : base(target, value1, value2)
    { }

    public override string ToString() => Target + " = " + Operands[0] + " + " + Operands[1];
}
