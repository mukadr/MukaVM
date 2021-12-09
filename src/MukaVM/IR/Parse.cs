using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MukaVM.IR
{
    public class Parse
    {
        private enum TokenKind
        {
            Identifier,
            Int,
            Function,
            If,
            Jmp,
            Ret,
            Colon,

            Plus,
            GreaterThan,
            Assign,
            OpenBrace,
            CloseBrace,

            EndOfSource,
            Invalid
        }

        private class Token
        {
            public TokenKind Kind { get; init; }
            public string Value { get; init; } = string.Empty;
        }

        private class Scanner
        {
            private readonly string _sourceText;
            private int _position;

            private Dictionary<string, TokenKind> _reservedWords = new()
            {
                { "FUNCTION", TokenKind.Function },
                { "IF", TokenKind.If },
                { "JMP", TokenKind.Jmp },
                { "RET", TokenKind.Ret }
            };

            public Scanner(string sourceText)
            {
                _sourceText = sourceText;
                _position = 0;
            }

            private int Look => (_position < _sourceText.Length ? _sourceText[_position] : -1);

            private void Advance() => _position++;

            private static bool IsWhitespace(int c) => (c == ' ' || c == '\t' || c == '\r' || c == '\n');

            private static bool IsAlpha(int c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');

            private static bool IsDigit(int c) => (c >= '0' && c <= '9');

            private void SkipWhitespace()
            {
                while (IsWhitespace(Look))
                {
                    Advance();
                }
            }

            private Token? TryScanIdentifier()
            {
                if (IsAlpha(Look) || Look == '_')
                {
                    var sb = new StringBuilder();

                    do
                    {
                        sb.Append((char)Look);
                        Advance();
                    } while (IsAlpha(Look) || Look == '_' || IsDigit(Look));

                    var value = sb.ToString();
                    if (_reservedWords.TryGetValue(value, out var reservedWord))
                    {
                        return CreateToken(reservedWord);
                    }
                    return CreateToken(TokenKind.Identifier, value);
                }

                return null;
            }

            private Token? TryScanNumber()
            {
                if (IsDigit(Look))
                {
                    var sb = new StringBuilder();

                    do
                    {
                        sb.Append((char)Look);
                        Advance();
                    } while (IsDigit(Look));

                    return CreateToken(TokenKind.Int, sb.ToString());
                }

                return null;
            }

            private Token? TryScanOperator()
            {
                switch (Look)
                {
                    case '+':
                        Advance();
                        return CreateToken(TokenKind.Plus);

                    case '>':
                        Advance();
                        return CreateToken(TokenKind.GreaterThan);

                    case '=':
                        Advance();
                        return CreateToken(TokenKind.Assign);

                    case '{':
                        Advance();
                        return CreateToken(TokenKind.OpenBrace);

                    case '}':
                        Advance();
                        return CreateToken(TokenKind.CloseBrace);

                    case ':':
                        Advance();
                        return CreateToken(TokenKind.Colon);

                    default:
                        return null;
                }
            }

            public Token NextToken()
            {
                SkipWhitespace();

                var token =
                    TryScanIdentifier() ??
                    TryScanNumber() ??
                    TryScanOperator();

                if (token is null)
                {
                    if (Look == -1)
                    {
                        token = CreateToken(TokenKind.EndOfSource);
                    }
                    else
                    {
                        Advance();
                        token = CreateToken(TokenKind.Invalid);
                    }
                }

                return token;
            }

            private Token CreateToken(TokenKind kind, string value = "") => new() { Kind = kind, Value = value };
        }

        public class ParseException : Exception
        {
            public ParseException(string message)
                : base("Parse error: " + message)
            { }
        }

        private sealed class JmpTarget : Label
        {
            public JmpTarget(string name)
                : base(name)
            { }
        }

        public static Function FromSourceText(string sourceText)
        {
            var scanner = new Scanner(sourceText);
            var token = scanner.NextToken();

            Token Match(Token current, params TokenKind[] expected)
            {
                if (!expected.Contains(current.Kind))
                {
                    throw new ParseException($"Expected {expected[0]} got {current.Kind}.");
                }
                return scanner.NextToken();
            }

            token = Match(token, TokenKind.Function);

            var function = new Function(token.Value);

            token = Match(token, TokenKind.Identifier);
            token = Match(token, TokenKind.OpenBrace);

            var variables = new List<Var>();
            var labels = new List<Label>();
            var jmpTargets = new List<JmpTarget>();

            Value GetValue(Token token)
            {
                if (token.Kind == TokenKind.Int)
                {
                    return new Int(token.Value);
                }
                return variables.Single(v => v.Name == token.Value);
            }

            while (token.Kind != TokenKind.CloseBrace &&
                   token.Kind != TokenKind.EndOfSource)
            {
                switch (token.Kind)
                {
                    case TokenKind.Identifier:
                        {
                            var id = token.Value;

                            var @var = variables.SingleOrDefault(v => v.Name == id);
                            if (@var is null)
                            {
                                @var = new Var(id);
                                variables.Add(@var);
                            }

                            token = Match(token, TokenKind.Identifier);

                            if (token.Kind == TokenKind.Assign)
                            {
                                var left = Match(token, TokenKind.Assign);
                                var plus = Match(left, TokenKind.Int, TokenKind.Identifier);
                                var right = Match(plus, TokenKind.Plus);
                                token = Match(right, TokenKind.Int, TokenKind.Identifier);

                                function.Instructions.Add(new Add(@var, GetValue(left), GetValue(right)));
                            }
                            else
                            {
                                var label = new Label(id);

                                if (labels.Any(l => l.Name == label.Name))
                                {
                                    throw new ParseException($"Label {label} already declared.");
                                }

                                labels.Add(label);
                                function.Instructions.Add(label);
                            }
                        }
                        break;

                    case TokenKind.Jmp:
                        {
                            token = Match(token, TokenKind.Jmp);
                            var target = jmpTargets.SingleOrDefault(t => t.Name == token.Value);
                            if (target is null)
                            {
                                target = new JmpTarget(token.Value);
                                jmpTargets.Add(target);
                            }

                            token = Match(token, TokenKind.Identifier);
                            function.Instructions.Add(new Jmp(target));
                        }
                        break;

                    case TokenKind.If:
                        {
                            var left = Match(token, TokenKind.If);
                            var op = Match(left, TokenKind.Int, TokenKind.Identifier);
                            var right = Match(op, TokenKind.GreaterThan);
                            token = Match(right, TokenKind.Int, TokenKind.Identifier);
                            token = Match(token, TokenKind.Colon);

                            var target = jmpTargets.SingleOrDefault(t => t.Name == token.Value);
                            if (target is null)
                            {
                                target = new JmpTarget(token.Value);
                                jmpTargets.Add(target);
                            }

                            token = Match(token, TokenKind.Identifier);
                            function.Instructions.Add(new Jg(GetValue(left), GetValue(right), target));
                        }
                        break;

                    case TokenKind.Ret:
                        function.Instructions.Add(new Ret());
                        token = Match(token, TokenKind.Ret);
                        break;

                    case TokenKind.Invalid:
                        throw new ParseException("Invalid token.");
                }
            }

            token = Match(token, TokenKind.CloseBrace);

            // Resolve jmp targets
            foreach (var instruction in function.Instructions)
            {
                if (instruction is Jmp jmp)
                {
                    var target = labels.Single(l => l.Name == jmp.Target.Name);
                    jmp.Target = target;
                }
            }

            return function;
        }
    }
}