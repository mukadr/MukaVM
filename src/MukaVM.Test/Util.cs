using MukaVM.IR;
using Xunit;

namespace MukaVM.Test
{
    internal static class Util
    {
        internal static void AssertControlFlowGraphEquals(string expected, Function actual)
        {
            AssertSourceEquals(expected, Convert.ToControlFlowGraph(actual).ToString());
        }

        internal static void AssertSourceEquals(string expected, string actual)
        {
            Assert.Equal(Format.FormatSource(expected), Format.FormatSource(actual));
        }
    }
}
