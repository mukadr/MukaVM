using System;

namespace MukaVM.IR;

public abstract class Instruction { }

public abstract class InstructionWithOperands(params Value[] operands) : Instruction
{
    public Value[] Operands { get; set; } = operands;

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

public abstract class InstructionWithTarget(Var target, params Value[] operands) : InstructionWithOperands(operands)
{
    public Var Target { get; set; } = target;
}
