using MukaVM.Gen.Amd64;
using MukaVM.IR;
using Xunit;

namespace MukaVM.Test
{
    internal static class Util
    {
        internal static void AssertControlFlowGraphEquals(string expected, string actual)
        {
            AssertSourceEquals(expected, CFG.Convert(Parse.FromSourceText(actual)).ToString());
        }

        internal static void AssertSSAEquals(string expected, string actual)
        {
            var cfgFunction = CFG.Convert(Parse.FromSourceText(actual));

            SSA.Transform(cfgFunction);

            AssertSourceEquals(expected, cfgFunction.ToString());
        }

        internal static void AssertAmd64Equals(string expected, string actual)
        {
            var cfgFunction = CFG.Convert(Parse.FromSourceText(actual));

            SSA.Transform(cfgFunction);

            RegAlloc.Transform(cfgFunction);

            AssertSourceEquals(expected, cfgFunction.ToString());
        }

        internal static void AssertSourceEquals(string expected, string actual)
        {
            Assert.Equal(Format.FormatSource(expected), Format.FormatSource(actual));
        }
    }
}
