namespace MukaVM.IR.Instructions;

public class Label(string name) : Instruction
{
    public string Name { get; set; } = name;

    public override string ToString() => Name;
}
