using Scheme.Compiler;

using static Scheme.Compiler.Match;

namespace Scheme.Tests.Compiler;

public class MatcherTests
{
    private readonly Parser _parser = new(false);

    [Fact]
    public void Match_OneOf()
    {
        var datum = _parser.Parse("a");
        var a = MatchOrThrow(datum, OneOf(Symbol("b"), Symbol("a")));
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }

    [Fact]
    public void Match_OneOf_Failure()
    {
        var datum = _parser.Parse("c");
        Assert.False(TryMatch(datum, OneOf(Symbol("b"), Symbol("a")), out _));
    }

    [Fact]
    public void Match_AnyDatum()
    {
        var datum = _parser.Parse("a");
        var a = MatchOrThrow(datum, AnyDatum);
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }

    [Fact]
    public void Match_AnyBoolean()
    {
        var datum = _parser.Parse("#t");
        var a = MatchOrThrow(datum, AnyBoolean);
        Assert.Equal(SchemeBoolean.True, a);
    }

    [Fact]
    public void Match_AnyBoolean_Failure()
    {
        var datum = _parser.Parse("3");
        TryMatch(datum, AnyBoolean, out _);
    }

    [Fact]
    public void Match_AnyNumber()
    {
        var datum = _parser.Parse("42");
        var a = MatchOrThrow(datum, AnyNumber);
        Assert.Equal(SchemeNumber.Wrap(42), a);
    }

    [Fact]
    public void Match_AnyString()
    {
        var datum = _parser.Parse("\"Hello World\"");
        var a = MatchOrThrow(datum, AnyString);
        Assert.Equal(new SchemeString("Hello World"), a);
    }

    [Fact]
    public void Match_AnySymbol()
    {
        var datum = _parser.Parse("a");
        var a = MatchOrThrow(datum, AnySymbol);
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }

    [Fact]
    public void Match_AnyPair()
    {
        var datum = _parser.Parse("(a . b)");
        var a = MatchOrThrow(datum, AnyPair);
        Assert.Equal(SchemeSymbol.FromString("a"), a.Car);
        Assert.Equal(SchemeSymbol.FromString("b"), a.Cdr);
    }

    [Fact]
    public void Match_AnyEmptyList()
    {
        var datum = _parser.Parse("()");
        var a = MatchOrThrow(datum, AnyEmptyList);
        Assert.Equal(SchemeEmptyList.Value, a);
    }

    [Fact]
    public void Match_AnyBytevector()
    {
        var datum = _parser.Parse("#u8(1)");
        var a = MatchOrThrow(datum, AnyBytevector);
        Assert.Equal(new SchemeBytevector(new byte[] { 1 }), a);
    }

    [Fact]
    public void Match_AnyVector()
    {
        var datum = _parser.Parse("#(a)");
        var a = MatchOrThrow(datum, AnyVector);
        Assert.Equal(new SchemeVector(new SchemeObject[] { SchemeSymbol.FromString("a") }), a);
    }

    [Fact]
    public void Match_Symbol_ByName()
    {
        var datum = _parser.Parse("a");
        var a = MatchOrThrow(datum, Symbol("a"));
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }

    [Fact]
    public void Match_Symbol_BySymbol()
    {
        var datum = _parser.Parse("a");
        var a = MatchOrThrow(datum, Symbol(SchemeSymbol.FromString("a")));
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }

    [Fact]
    public void Match_Pair()
    {
        var datum = _parser.Parse("(1 . 2)");
        var (p, car, cdr) = MatchOrThrow(datum, Pair());
        Assert.Equal(car, 1);
        Assert.Equal(cdr, 2);
    }

    [Fact]
    public void Match_List()
    {
        var datum = _parser.Parse("(1 2 3)");
        var (p, xs) = MatchOrThrow(datum, List());
        Assert.Equal(new SchemeObject[] { 1, 2, 3 }, xs);
    }

    [Fact]
    public void Match_ListOf()
    {
        var datum = _parser.Parse("(1 2 3)");
        var (p, xs) = MatchOrThrow(datum, ListMany(AnyNumber));
        Assert.Equal(new SchemeObject[] { 1, 2, 3 }, xs);
    }
    [Fact]
    public void Match_ListMany_Fail()
    {
        var datum = _parser.Parse("(1 a 3)");
        Assert.False(TryMatch(datum, ListMany(AnyNumber), out _));
    }

    [Fact]
    public void Match_ImproperList()
    {
        var datum = _parser.Parse("(1 . (2 . 3))");
        var (p, xs, x) = MatchOrThrow(datum, ImproperList());
        Assert.Equal(new SchemeObject[] { 1, 2 }, xs);
        Assert.Equal(3, x);
    }

    [Fact]
    public void Match_List_One()
    {
        var datum = _parser.Parse("(a)");
        var (p, a) = MatchOrThrow(datum, List(Symbol("a")));
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }

    [Fact]
    public void Match_List_Two()
    {
        var datum = _parser.Parse("(a b)");

        var (_, a, b) = MatchOrThrow(datum, List(Symbol("a"), Symbol("b")));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
    }

    [Fact]
    public void Match_List_Three()
    {
        var datum = _parser.Parse("(a b c)");

        var (_, a, b, c) = MatchOrThrow(datum, List(Symbol("a"), Symbol("b"), Symbol("c")));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
        Assert.Equal(SchemeSymbol.FromString("c"), c);
    }

    [Fact]
    public void Match_List_Four()
    {
        var datum = _parser.Parse("(a b c d)");

        var (_, a, b, c, d) = MatchOrThrow(datum, List(Symbol("a"), Symbol("b"), Symbol("c"), Symbol("d")));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
        Assert.Equal(SchemeSymbol.FromString("c"), c);
        Assert.Equal(SchemeSymbol.FromString("d"), d);
    }

    [Fact]
    public void Match_ListMany_One()
    {
        var datum = _parser.Parse("(a b c)");
        var (p, a, xs) = MatchOrThrow(datum, ListMany(Symbol("a"), AnySymbol));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(2, xs.Length);
        Assert.Equal(SchemeSymbol.FromString("b"), xs[0]);
        Assert.Equal(SchemeSymbol.FromString("c"), xs[1]);
    }

    [Fact]
    public void Match_ListMany_Two()
    {
        var datum = _parser.Parse("(a b c d)");

        var (_, a, b, xs) = MatchOrThrow(datum, ListMany(Symbol("a"), Symbol("b"), AnySymbol));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
        Assert.Equal(2, xs.Length);
        Assert.Equal(SchemeSymbol.FromString("c"), xs[0]);
        Assert.Equal(SchemeSymbol.FromString("d"), xs[1]);
    }

    [Fact]
    public void Match_ListMany_Three()
    {
        var datum = _parser.Parse("(a b c d e)");

        var (_, a, b, c, xs) = MatchOrThrow(datum, ListMany(Symbol("a"), Symbol("b"), Symbol("c"), AnySymbol));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
        Assert.Equal(SchemeSymbol.FromString("c"), c);
        Assert.Equal(2, xs.Length);
        Assert.Equal(SchemeSymbol.FromString("d"), xs[0]);
        Assert.Equal(SchemeSymbol.FromString("e"), xs[1]);
    }

    [Fact]
    public void Match_ListMany_Four()
    {
        var datum = _parser.Parse("(a b c d e f)");

        var (_, a, b, c, d, xs) = MatchOrThrow(datum, ListMany(Symbol("a"), Symbol("b"), Symbol("c"), Symbol("d"), AnySymbol));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
        Assert.Equal(SchemeSymbol.FromString("c"), c);
        Assert.Equal(SchemeSymbol.FromString("d"), d);
        Assert.Equal(2, xs.Length);
        Assert.Equal(SchemeSymbol.FromString("e"), xs[0]);
        Assert.Equal(SchemeSymbol.FromString("f"), xs[1]);
    }

    [Fact]
    public void Match_Nested()
    {
        var datum = _parser.Parse("(a (b c))");

        var (_, a, (_, b, c)) = MatchOrThrow(datum,
            List(Symbol("a"),
                List(Symbol("b"), Symbol("c"))));

        Assert.Equal(SchemeSymbol.FromString("a"), a);
        Assert.Equal(SchemeSymbol.FromString("b"), b);
        Assert.Equal(SchemeSymbol.FromString("c"), c);
    }

    [Fact]
    public void Match_Syntax()
    {
        var syntaxParser = new Parser(true);
        var o = syntaxParser.Parse("(a)");

        var (p, (stx, a)) = MatchOrThrow(o, List(Syntax(Symbol("a"))));
        Assert.Equal(SchemeSymbol.FromString("a"), a);
    }
}