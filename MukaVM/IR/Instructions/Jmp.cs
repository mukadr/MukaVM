namespace MukaVM.IR.Instructions;

public abstract class JmpInstruction(Label target, params Value[] operands) : InstructionWithOperands(operands)
{
    public Label Target { get; set; } = target;
}

public class Jmp(Label target, params Value[] values) : JmpInstruction(target, values)
{
    public override string ToString() => "JMP " + Target;

    protected string ToConditionalJmpString(string op) => "IF " + Operands[0] + $" {op} " + Operands[1] + ": " + Target;
}

public class Je(Value value1, Value value2, Label target) : Jmp(target, value1, value2)
{
    public override string ToString() => ToConditionalJmpString("=");
}

public class Jne(Value value1, Value value2, Label target) : Jmp(target, value1, value2)
{
    public override string ToString() => ToConditionalJmpString("!=");
}

public class Jg(Value value1, Value value2, Label target) : Jmp(target, value1, value2)
{
    public override string ToString() => ToConditionalJmpString(">");
}

public class Jl(Value value1, Value value2, Label target) : Jmp(target, value1, value2)
{
    public override string ToString() => ToConditionalJmpString("<");
}

public class Jge(Value value1, Value value2, Label target) : Jmp(target, value1, value2)
{
    public override string ToString() => ToConditionalJmpString(">=");
}

public class Jle(Value value1, Value value2, Label target) : Jmp(target, value1, value2)
{
    public override string ToString() => ToConditionalJmpString("<=");
}