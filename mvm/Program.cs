using MukaVM.IR;

if (args.Length < 1)
{
    Console.WriteLine("Usage: mvm <source-file>");
    return;
}

var sourceText = File.ReadAllText(args[0]);

var cfg = MukaVM.IR.CFG.Transform.ToControlFlowGraph(Parse.FromSourceText(sourceText));
MukaVM.IR.SSA.Transform.ToSSAForm(cfg);

Console.WriteLine(Format.FormatSource(cfg.ToString()));