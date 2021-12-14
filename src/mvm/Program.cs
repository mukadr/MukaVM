using MukaVM.IR;

if (args.Length < 1)
{
    Console.WriteLine("Usage: mvm <source-file>");
    return;
}

var sourceText = File.ReadAllText(args[0]);

var cfg = CFG.Convert(Parse.FromSourceText(sourceText));
SSA.Transform(cfg);

Console.WriteLine(cfg);