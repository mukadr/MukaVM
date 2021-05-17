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

       public override string ToString() => Name;
    }

    public class Int : Value
    {
        public int Value { get; set; }

        public Int(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
