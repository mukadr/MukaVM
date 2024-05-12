using MukaVM.IR.Instructions;

namespace MukaVM.IR.CFG;

public class CfgLabel : Label
{
    public BasicBlock BasicBlock { get; set; }

    public CfgLabel(BasicBlock basicBlock)
        : base(basicBlock.Name)
    {
        BasicBlock = basicBlock;
    }
}