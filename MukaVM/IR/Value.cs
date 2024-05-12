namespace MukaVM.IR;

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

public class SSAVar : Var
{
    public Var Var { get; }

    public int N { get; }

    public SSAVar(int n, Var var)
        : base("v" + n.ToString())
    {
        N = n;
        Var = var;
    }
}

public class Int : Value
{
    public int Value { get; set; }

    public Int(int value)
    {
        Value = value;
    }

    public Int(string value)
    {
        Value = int.Parse(value);
    }

    public override string ToString() => Value.ToString();
}