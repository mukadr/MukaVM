using System;

namespace MukaVM.IR.Instructions;

public abstract class Instruction { }

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
