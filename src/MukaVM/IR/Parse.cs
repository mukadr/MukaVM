using ParseSharp;
using static ParseSharp.Parser;
using System.Collections.Generic;
using System.Linq;

namespace MukaVM.IR;

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
        var variables = new List<Var>();
        var labels = new List<Label>();
        var jmpTargets = new List<JmpTarget>();

        Var FindVariable(string name)
        {
            var @var = variables.SingleOrDefault(v => v.Name == name);
            if (@var is null)
            {
                throw new ParserException($"Undeclared variable {name}.");
            }
            return @var;
        }

        Var FindOrCreateVariable(string name)
        {
            var @var = variables.SingleOrDefault(v => v.Name == name);
            if (@var is null)
            {
                @var = new Var(name);
                variables.Add(@var);
            }
            return @var;
        }

        Label CreateLabel(string labelName)
        {
            if (labels.Any(l => l.Name == labelName))
            {
                throw new ParserException($"Label {labelName} already declared.");
            }
            var label = new Label(labelName);
            labels.Add(label);
            return label;
        }

        JmpTarget FindOrCreateJmpTarget(string labelName)
        {
            var target = jmpTargets.SingleOrDefault(t => t.Name == labelName);
            if (target is null)
            {
                target = new JmpTarget(labelName);
                jmpTargets.Add(target);
            }
            return target;
        }

        List<Instruction> UpdateJmpTargets(List<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction is Jmp jmp)
                {
                    var target = labels.SingleOrDefault(l => l.Name == jmp.Target.Name);
                    if (target is null)
                    {
                        throw new ParserException($"Label {jmp.Target.Name} not found.");
                    }
                    jmp.Target = target;
                }
            }
            return instructions;
        }

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
        var eq = Token("=");
        var ne = Token("!=");
        var gt = Token(">");
        var lt = Token("<");
        var ge = Token(">=");
        var le = Token("<=");
        var conditional = eq.Or(ne).Or(ge).Or(le).Or(gt).Or(lt);
        var colon = Token(":");
        var openBrace = Token("{");
        var closeBrace = Token("}");

        var argument =
            integer.Map<Value>(i => new Int(i.Value))
            .Or(identifier.Map<Value>(id => FindVariable(id.Value)));

        var movInstruction =
            identifier.Bind(id =>
                assign.And(argument.Map<Instruction>(value =>
                    new Mov(FindOrCreateVariable(id.Value), value))));

        var addInstruction =
            identifier.Bind(id =>
                assign.And(argument).Bind(left =>
                    plus.And(argument).Map<Instruction>(right =>
                        new Add(FindOrCreateVariable(id.Value), left, right))));

        var labelInstruction =
            identifier.Map<Instruction>(labelName =>
                CreateLabel(labelName.Value));

        var jmpInstruction =
            jmpKw.And(identifier.Map<Instruction>(labelName =>
                new Jmp(FindOrCreateJmpTarget(labelName.Value))));

        var ifInstruction =
            ifKw.And(argument.Bind(left =>
                conditional.Bind(op =>
                    argument.Bind(right =>
                        colon.And(identifier.Map<Instruction>(labelName =>
                            InstructionWithOperands.CreateIfInstruction(
                                left, op.Value, right, FindOrCreateJmpTarget(labelName.Value))))))));

        var retInstruction =
            retKw.Map<Instruction>(_ => new Ret());

        var instruction =
            jmpInstruction
            .Or(ifInstruction)
            .Or(retInstruction)
            .Or(addInstruction)
            .Or(movInstruction)
            .Or(labelInstruction);

        var functionDefinition =
            Optional(Whitespace)
            .And(functionKw.And(identifier.Bind(name =>
                openBrace.And(ZeroOrMore(instruction).Bind(instructions =>
                    closeBrace.Map(_ =>
                        new Function(name.Value, UpdateJmpTargets(instructions))))))));

        return functionDefinition.ParseAllText(sourceText);
    }
}