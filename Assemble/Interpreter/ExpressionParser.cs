using Assemble.Interpreter.AST;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Assemble.Interpreter;

public static class ExpressionParser
{
    public static Expression Parse(string input)
    {
        return SExpression.ParseOrThrow(input);
    }

    private static Parser<char, T> Token<T>(Parser<char, T> p)
        => Try(p).Before(SkipWhitespaces);

    private static Parser<char, char> Token(char value)
        => Token(Char(value));

    private static Parser<char, string> Token(string value)
        => Token(String(value));




    private static readonly Parser<char, char> ExtendedIdentifierCharacter =
        OneOf("!$%&*+-./:<=>?@^_~");


    private static Parser<char, AST.Expression> SExpression =>
        from _ in SkipWhitespaces
        from e in OneOf(SQuoted, SBoolean, SString, SNumber, SList, SAtom)
        select e;

    private static Parser<char, AST.Expression> SString => Token(
        from v in AnyCharExcept('"').ManyString().Between(Char('"'))
        select (AST.Expression)new AST.String() { Value = v }
    ).Labelled("string");

    private static Parser<char, AST.Expression> SNumber => Token(
        from w in Digit.AtLeastOnceString()
        from d in Try(Char('.').Then(Digit.AtLeastOnceString())).Optional()
        select (AST.Expression)new AST.Number(decimal.Parse(
            d.HasValue ? $"{w}.{d.Value}" : w))
    ).Labelled("number");

    private static Parser<char, AST.Expression> SAtom => Token(
        from x in Letter.Or(ExtendedIdentifierCharacter)
        from xs in OneOf(Letter, Digit, ExtendedIdentifierCharacter).ManyString()
        select (AST.Expression)new AST.Atom(x + xs)
    ).Labelled("atom");

    private static Parser<char, AST.Expression> SList => Token(
        from xs in SExpression.Many().Between(Char('('), Char(')'))
        select (AST.Expression)new AST.List() { Elements = xs.ToArray() }
    ).Labelled("list");

    private static Parser<char, AST.Expression> SQuoted => Token(
        from _ in Char('\'')
        from e in SExpression
        select (AST.Expression)new AST.List()
        {
            Elements = new AST.Expression[]
            {
                AST.Atom.Quote,
                e
            }
        }
    ).Labelled("quoted expression");

    private static Parser<char, AST.Expression> SBoolean => Token(
        from _ in Char('#')
        from v in OneOf(Char('t'), Char('f'))
        select (AST.Expression)new AST.Boolean(v == 't')
    ).Labelled("boolean");

    private static Parser<char, AST.Expression> SDottedList => Token(
        from xs in SExpression.AtLeastOnce()
        from x in SExpression
        select (AST.Expression)new AST.DottedList() { Head = xs.ToArray(), Tail = x }
    );
}