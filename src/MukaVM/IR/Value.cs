namespace MukaVM.IR
{
    public abstract class Value { }

    public class Var : Value
    {
        public string Name { get; set; }

        public Var(string name)
        {
            Name = name;
        }

        public override bool Equals(object? obj)
            => obj is Var var &&
               var.GetType() == typeof(Var) &&
               var.Name == Name;

        public override int GetHashCode()
            => Name.GetHashCode();
    }

    public sealed class Int : Value
    {
        public int Value { get; set; }

        public Int(int value)
        {
            Value = value;
        }

        public override bool Equals(object? obj)
            => obj is Int i &&
               i.Value == Value;

        public override int GetHashCode()
            => Value.GetHashCode();
    }

    public sealed class Str : Value
    {
        public string Value { get; set; }

        public Str(string value)
        {
            Value = value;
        }

        public override bool Equals(object? obj)
            => obj is Str s &&
               s.Value == Value;

        public override int GetHashCode()
            => Value.GetHashCode();
    }
}
