namespace MukaVM.IR
{
    public abstract class Instruction { }

    public class Add : Instruction
    {
        public Var Target { get; set; }
        public Value Value1 { get; set; }
        public Value Value2 { get; set; }

        public Add(Var target, Value value1, Value value2)
        {
            Target = target;
            Value1 = value1;
            Value2 = value2;
        }

        public override string ToString() => Target + " = " + Value1 + " + " + Value2;
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

    public class Jmp : Instruction
    {
        public Label Target { get; set; }

        public Jmp(Label target)
        {
            Target = target;
        }

        public override string ToString() => "JMP " + Target;
    }

    public class Jg : Jmp
    {
        public Value Value1 { get; set; }
        public Value Value2 { get; set; }

        public Jg(Value value1, Value value2, Label target)
            : base(target)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public override string ToString() => "JG " + Value1 + ", " + Value2 + ", " + Target;
    }

    public class Ret : Instruction
    {
        public override string ToString() => "RET";
    }
}
