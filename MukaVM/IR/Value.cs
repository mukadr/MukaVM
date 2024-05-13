namespace MukaVM.IR;

public abstract class Value { }

public class Var(string name) : Value
{
    public string Name { get; set; } = name;

    public override string ToString() => Name;
}

public class SSAVar(int n, Var var) : Var("v" + n.ToString())
{
    public Var Var { get; } = var;

    public int N { get; } = n;
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