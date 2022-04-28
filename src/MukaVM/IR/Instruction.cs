using System;
using System.Collections.Generic;

namespace MukaVM.IR;

public abstract class Instruction { }

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

public abstract class InstructionWithOperands : Instruction
{
    public Value[] Operands { get; set; }

    public InstructionWithOperands(params Value[] operands)
    {
        Operands = operands;
    }

    public static InstructionWithOperands CreateIfInstruction(Value left, string op, Value right, Label target)
    {
        return op switch
        {
            "=" => new Je(left, right, target),
            "!=" => new Jne(left, right, target),
            ">" => new Jg(left, right, target),
            "<" => new Jl(left, right, target),
            ">=" => new Jge(left, right, target),
            "<=" => new Jle(left, right, target),
            _ => throw new ArgumentException(op),
        };
    }
}

public abstract class InstructionWithTarget : InstructionWithOperands
{
    public Var Target { get; set; }

    public InstructionWithTarget(Var target, params Value[] operands)
        : base(operands)
    {
        Target = target;
    }
}

public class Mov : InstructionWithTarget
{
    public Mov(Var target, Value value)
        : base(target, value)
    { }

    public override string ToString() => Target + " = " + Operands[0];
}

public class Add : InstructionWithTarget
{
    public Add(Var target, Value value1, Value value2)
        : base(target, value1, value2)
    { }

    public override string ToString() => Target + " = " + Operands[0] + " + " + Operands[1];
}

public class Label : Instruction
{
    public string Name { get; set; }

    public Label(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}

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

public class Ret : Instruction
{
    public override string ToString() => "RET";
}