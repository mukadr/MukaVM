namespace MukaVM.IR.Instructions;

public class Label : Instruction
{
    public string Name { get; set; }

    public Label(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}
