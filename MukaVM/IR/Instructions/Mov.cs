namespace MukaVM.IR.Instructions;

public class Mov : InstructionWithTarget
{
    public Mov(Var target, Value value)
        : base(target, value)
    { }

    public override string ToString() => Target + " = " + Operands[0];
}
