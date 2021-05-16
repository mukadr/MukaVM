namespace MukaVM.IR
{
    public abstract class Instruction { }

    public sealed class Add : Instruction
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

        public override bool Equals(object? obj)
            => obj is Add add &&
               add.Target.Equals(Target) &&
               add.Value1.Equals(Value1) &&
               add.Value2.Equals(Value2);

        public override int GetHashCode()
            => Target.GetHashCode() ^ Value1.GetHashCode() ^ Value2.GetHashCode();
    }

    public class Label : Instruction
    {
        public string Name { get; set; }

        public Label(string name)
        {
            Name = name;
        }

        public override bool Equals(object? obj)
            => obj is Label label &&
               label.GetType() == typeof(Label) &&
               label.Name == Name;

        public override int GetHashCode()
            => Name.GetHashCode();
    }

    public class Jmp : Instruction
    {
        public Label Target { get; set; }

        public Jmp(Label target)
        {
            Target = target;
        }

        public override bool Equals(object? obj)
            => obj is Jmp jmp &&
               jmp.GetType() == typeof(Jmp) &&
               jmp.Target.Equals(Target);

        public override int GetHashCode()
            => Target.GetHashCode();
    }

    public sealed class Jg : Jmp
    {
        public Value Value1 { get; set; }
        public Value Value2 { get; set; }

        public Jg(Value value1, Value value2, Label target)
            : base(target)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public override bool Equals(object? obj)
            => obj is Jg jg &&
               jg.Target.Equals(Target) &&
               jg.Value1.Equals(Value1) &&
               jg.Value2.Equals(Value2);

        public override int GetHashCode()
            => Target.GetHashCode() ^ Value1.GetHashCode() ^ Value2.GetHashCode();
    }

    public sealed class Ret : Instruction
    {
        public override bool Equals(object? obj)
            => obj is Ret;

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
