namespace MukaVM.IR.Instructions;

public abstract class JmpInstruction : InstructionWithOperands
{
    public Label Target { get; set; }

    public JmpInstruction(Label target, params Value[] operands)
        : base(operands)
    {
        Target = target;
    }
}

public class Jmp : JmpInstruction
{
    public Jmp(Label target, params Value[] values)
        : base(target, values)
    { }

    public override string ToString() => "JMP " + Target;
}


public class Je : Jmp
{
    public Je(Value value1, Value value2, Label target)
        : base(target, value1, value2)
    { }

    public override string ToString() => "IF " + Operands[0] + " = " + Operands[1] + ": " + Target;
}

public class Jne : Jmp
{
    public Jne(Value value1, Value value2, Label target)
        : base(target, value1, value2)
    { }

    public override string ToString() => "IF " + Operands[0] + " != " + Operands[1] + ": " + Target;
}

public class Jg : Jmp
{
    public Jg(Value value1, Value value2, Label target)
        : base(target, value1, value2)
    { }

    public override string ToString() => "IF " + Operands[0] + " > " + Operands[1] + ": " + Target;
}

public class Jl : Jmp
{
    public Jl(Value value1, Value value2, Label target)
        : base(target, value1, value2)
    { }

    public override string ToString() => "IF " + Operands[0] + " < " + Operands[1] + ": " + Target;
}

public class Jge : Jmp
{
    public Jge(Value value1, Value value2, Label target)
        : base(target, value1, value2)
    { }

    public override string ToString() => "IF " + Operands[0] + " >= " + Operands[1] + ": " + Target;
}

public class Jle : Jmp
{
    public Jle(Value value1, Value value2, Label target)
        : base(target, value1, value2)
    { }

    public override string ToString() => "IF " + Operands[0] + " <= " + Operands[1] + ": " + Target;
}
