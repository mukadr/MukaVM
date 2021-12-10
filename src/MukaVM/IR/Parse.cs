using ParseSharp;
using static ParseSharp.Parser;
using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR
{
    public class Parse
    {
        private sealed class JmpTarget : Label
        {
            public JmpTarget(string name)
                : base(name)
            { }
        }

        public static Function FromSourceText(string sourceText)
        {
            var letter = Match('a', 'z').Or(Match('A', 'Z'));
            var digit = Match('0', '9');
            var integer = Token(OneOrMore(digit));
            var identifier = Token(letter.Bind(l => ZeroOrMore(letter.Or(digit)).Map(ld => l + ld)));
            var functionKw = Token("FUNCTION");
            var ifKw = Token("IF");
            var jmpKw = Token("JMP");
            var retKw = Token("RET");
            var assign = Token("=");
            var plus = Token("+");
            var gt = Token(">");
            var colon = Token(":");
            var openBrace = Token("{");
            var closeBrace = Token("}");

            var variables = new List<Var>();
            var labels = new List<Label>();
            var jmpTargets = new List<JmpTarget>();

            var argument =
                integer.Map<Value>(i => new Int(i.Value))
                .Or(identifier.Map<Value>(id => new Var(id.Value)));

            var addInstruction =
                identifier.Bind(id =>
                    assign.And(argument).Bind(left =>
                        plus.And(argument).Map<Instruction>(right =>
                        {
                            var v = variables.SingleOrDefault(v => v.Name == id.Value);
                            if (v is null)
                            {
                                v = new Var(id.Value);
                                variables.Add(v);
                            }
                            return new Add(v, left, right);
                        })));

            var labelInstruction =
                identifier.Map<Instruction>(id =>
                {
                    var l = new Label(id.Value);
                    if (labels.Any(ll => ll.Name == l.Name))
                    {
                        throw new ParserException($"Label {l} already declared.");
                    }
                    labels.Add(l);
                    return l;
                });

            var jmpInstruction =
                jmpKw.And(identifier.Map<Instruction>(id =>
                {
                    var target = jmpTargets.SingleOrDefault(t => t.Name == id.Value);
                    if (target is null)
                    {
                        target = new JmpTarget(id.Value);
                        jmpTargets.Add(target);
                    }
                    return new Jmp(target);
                }));

            var ifInstruction =
                ifKw.And(argument.Bind(left =>
                    gt.And(argument.Bind(right =>
                        colon.And(identifier.Map<Instruction>(labelName =>
                        {
                            var target = jmpTargets.SingleOrDefault(t => t.Name == labelName.Value);
                            if (target is null)
                            {
                                target = new JmpTarget(labelName.Value);
                                jmpTargets.Add(target);
                            }
                            return new Jg(left, right, target);
                        }))))));

            var retInstruction =
                retKw.Map<Instruction>(_ => new Ret());

            var instruction =
                jmpInstruction
                .Or(ifInstruction)
                .Or(retInstruction)
                .Or(addInstruction)
                .Or(labelInstruction);

            var functionDefinition =
                Optional(Whitespace)
                .And(functionKw.And(identifier.Bind(name =>
                    openBrace.And(ZeroOrMore(instruction).Bind(instructions =>
                        closeBrace.Map(_ =>
                        {
                            foreach (var instruction in instructions)
                            {
                                if (instruction is Jmp jmp)
                                {
                                    jmp.Target = labels.Single(l => l.Name == jmp.Target.Name);
                                }
                            }
                            return new Function(name.Value, instructions);
                        }))))));

            return functionDefinition.ParseAllText(sourceText);
        }
    }
}