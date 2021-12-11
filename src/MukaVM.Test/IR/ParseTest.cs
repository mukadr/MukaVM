using MukaVM.IR;
using ParseSharp;
using Xunit;

namespace MukaVM.Test.IR
{
    public class ParseTest
    {
        [Fact]
        public void FromSourceText_Parses_Ret()
        {
            const string sourceText = @"
                FUNCTION ret {
                    RET
                }";

            Util.AssertSourceEquals(sourceText, Parse.FromSourceText(sourceText).ToString());
        }

        [Fact]
        public void FromSourceText_Parses_Addition()
        {
            const string sourceText = @"
                FUNCTION simpleAddition {
                    x = 0 + 10
                }";

            Util.AssertSourceEquals(sourceText, Parse.FromSourceText(sourceText).ToString());
        }

        [Fact]
        public void FromSourceText_Parses_Labels()
        {
            const string sourceText = @"
                FUNCTION labels {
                    start
                    end
                }";

            Util.AssertSourceEquals(sourceText, Parse.FromSourceText(sourceText).ToString());
        }

        [Fact]
        public void FromSourceText_Parses_Jmps()
        {
            const string sourceText = @"
                FUNCTION jmps {
                    JMP end
                    x = 0 + 10
                    end
                }";

            Util.AssertSourceEquals(sourceText, Parse.FromSourceText(sourceText).ToString());
        }

        [Fact]
        public void FromSourceText_Parses_Variables()
        {
            const string sourceText = @"
                FUNCTION vars {
                    x = 0 + 10
                    y = x + 5
                }";

            Util.AssertSourceEquals(sourceText, Parse.FromSourceText(sourceText).ToString());
        }

        [Fact]
        public void FromSourceText_Parses_Ifs()
        {
            const string sourceText = @"
                FUNCTION jmps {
                    x = 0 + 8
                    IF x > 5: greater
                    greater
                }";

            Util.AssertSourceEquals(sourceText, Parse.FromSourceText(sourceText).ToString());
        }

        [Fact]
        public void FromSourceText_Throws_When_Variable_Not_Found()
        {
            const string sourceText = @"
                FUNCTION variableNotFound {
                    x = 0 + y
                }";

            Assert.Throws<ParserException>(() => Parse.FromSourceText(sourceText));
        }

        [Fact]
        public void FromSourceText_Throws_When_Label_Not_Found()
        {
            const string sourceText = @"
                FUNCTION labelNotFound {
                    JMP notFound
                }";

            Assert.Throws<ParserException>(() => Parse.FromSourceText(sourceText));
        }

        [Fact]
        public void FromSourceText_Throws_When_Label_Redeclared()
        {
            const string sourceText = @"
                FUNCTION redeclaredLabel {
                    lab
                    lab
                }";

            Assert.Throws<ParserException>(() => Parse.FromSourceText(sourceText));
        }
    }
}