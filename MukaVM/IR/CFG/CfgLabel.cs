using MukaVM.IR.Instructions;

namespace MukaVM.IR.CFG;

public class CfgLabel(BasicBlock basicBlock) : Label(basicBlock.Name)
{
    public BasicBlock BasicBlock { get; set; } = basicBlock;
}