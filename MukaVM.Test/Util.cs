using MukaVM.IR;
using Xunit;

namespace MukaVM.Test;

internal static class Util
{
    internal static void AssertControlFlowGraphEquals(string expected, string actual)
    {
        var cfgFunction = MukaVM.IR.CFG.Transform.ToControlFlowGraph(Parse.FromSourceText(actual));

        AssertSourceEquals(expected, cfgFunction.ConvertToString(true));
    }

    internal static void AssertSSAEquals(string expected, string actual)
    {
        var cfgFunction = MukaVM.IR.CFG.Transform.ToControlFlowGraph(Parse.FromSourceText(actual));

        MukaVM.IR.SSA.Transform.ToSSAForm(cfgFunction);

        AssertSourceEquals(expected, cfgFunction.ToString());
    }

    internal static void AssertSourceEquals(string expected, string actual)
    {
        Assert.Equal(Format.FormatSource(expected), Format.FormatSource(actual));
    }
}