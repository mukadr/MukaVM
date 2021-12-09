using MukaVM.IR;
using Xunit;

namespace MukaVM.Test
{
    internal static class Util
    {
        internal static void AssertControlFlowGraphEquals(string expected, Function actual)
        {
            AssertSourceEquals(expected, CfgBuilder.ToControlFlowGraph(actual).ToString());
        }

        internal static void AssertSSAEquals(string expected, Function actual)
        {
            var cfgFunction = CfgBuilder.ToControlFlowGraph(actual);

            SSA.Transform(cfgFunction);

            AssertSourceEquals(expected, cfgFunction.ToString());
        }

        internal static void AssertSourceEquals(string expected, string actual)
        {
            Assert.Equal(Format.FormatSource(expected), Format.FormatSource(actual));
        }
    }
}
