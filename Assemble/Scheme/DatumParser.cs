using System.Text;
using Pidgin;
using static Pidgin.Parser;

namespace Assemble.Scheme;

public static class DatumParser
{
    public static SchemeDatum Parse(string input)
    {
        return (
            from _ in SkipWhitespaces
            from e in OneOf(SimpleDatum, CompoundDatum)
            from _end in Parser<char>.End
            select e
        ).ParseOrThrow(input);
    }

    private static Parser<char, T> Token<T>(Parser<char, T> p)
        => Try(p).Before(SkipWhitespaces);

    private static Parser<char, SchemeDatum> Parser =>
        OneOf(SimpleDatum, CompoundDatum);

    #region Simple Datums

    private static Parser<char, SchemeDatum> SimpleDatum =>
        OneOf(BooleanParser, NumberParser, CharacterParser, StringParser, SymbolParser, ByteVectorParser);

    private static Parser<char, SchemeDatum> BooleanParser => Token(
        from _ in Char('#')
        from v in OneOf(
            Char('t').Before(Try(String("rue")).Optional()),
            Char('f').Before(Try(String("alse")).Optional()))
        select (SchemeDatum)SchemeBoolean.FromBoolean(v == 't')
    ).Labelled("boolean");

    private static decimal ParseDecimal(Maybe<char> sign, string wholePart, Maybe<string> decimalPart)
    {
        var sb = new StringBuilder();
        if (sign.HasValue)
            sb.Append(sign.Value);
        sb.Append(wholePart);
        if (decimalPart.HasValue)
        {
            sb.Append('.');
            sb.Append(decimalPart.Value);
        }
        return decimal.Parse(sb.ToString());
    }

    private static Parser<char, SchemeDatum> NumberParser => Token(
        from sign in Try(Char('-').Or(Char('+')).Optional())
        from w in Digit.AtLeastOnceString()
        from d in Try(Char('.').Then(Digit.AtLeastOnceString())).Optional()
        select (SchemeDatum)new SchemeNumber(ParseDecimal(sign, w, d))
    ).Labelled("number");

    private static Parser<char, char> CharacterNameParser =>
        from name in OneOf(
            Try(String(SchemeCharacter.Names.Alarm)),
            Try(String(SchemeCharacter.Names.Backspace)),
            Try(String(SchemeCharacter.Names.Delete)),
            Try(String(SchemeCharacter.Names.Escape)),
            Try(String(SchemeCharacter.Names.NewLine)),
            Try(String(SchemeCharacter.Names.Null)),
            Try(String(SchemeCharacter.Names.Return)),
            Try(String(SchemeCharacter.Names.Space)),
            Try(String(SchemeCharacter.Names.Tab))
        )
        select SchemeCharacter.Names.Lookup(name)!.Value;

    private static Parser<char, char> CharacterHexParser =>
        from _ in Char('x')
        from v in HexNum
        select (char)v;

    private static Parser<char, SchemeDatum> CharacterParser => Token(
        from _ in String("#\\")
        from v in OneOf(Try(CharacterNameParser), Try(CharacterHexParser), LetterOrDigit)
        select (SchemeDatum)new SchemeCharacter(v)
    ).Labelled("character");

    private static Parser<char, SchemeDatum> StringParser => Token(
        from v in AnyCharExcept('"').ManyString().Between(Char('"'))
        select (SchemeDatum)new SchemeString(v)
    ).Labelled("string");

    private static Parser<char, char> InitialParser =>
        LetterParser.Or(SpecialInitialParser);

    private static Parser<char, char> LetterParser =>
        Parser<char>.Token(char.IsAsciiLetter);

    private static Parser<char, char> SpecialInitialParser =>
        OneOf("!$%&*/:<=>?^_~");

    private static Parser<char, char> SubsequentParser =>
        InitialParser.Or(Digit).Or(SpecialSubsequentParser);

    private static Parser<char, char> SpecialSubsequentParser =>
        ExplicitSignParser.Or(OneOf("@."));

    private static Parser<char, char> ExplicitSignParser =>
        OneOf("+-");

    private static Parser<char, string> IdentifierParser =>
        from x in InitialParser
        from xs in SubsequentParser.ManyString()
        select x + xs;

    private static Parser<char, SchemeDatum> SymbolParser => Token(
        from n in IdentifierParser
        select (SchemeDatum)SchemeSymbol.FromString(n)
    ).Labelled("symbol");

    private static Parser<char, SchemeDatum> ByteVectorParser => Token(
        from xs in
            String("#u8").Then(
                ByteParser
                    .Separated(Whitespaces)
                .Between(Char('('), Char(')')))
        select (SchemeDatum)new SchemeBytevector(xs.ToArray())
    ).Labelled("bytevector");

    private static Parser<char, byte> ByteParser =>
        Digit.AtLeastOnceString().Select(x => byte.Parse(x));

    #endregion

    #region Compound Datums

    private static Parser<char, SchemeDatum> CompoundDatum =>
        OneOf(ListParser, VectorParser, AbbreviationParser);

    private static Parser<char, SchemeDatum> EmptyListParser =>
        Token(Whitespaces.Between(Char('('), Char(')')))
            .Select(_ => (SchemeDatum)SchemeEmptyList.Value);

    private static Parser<char, SchemeDatum> ListOrPairParser =>
        from _open in Token(Char('('))
        from cars in Parser.AtLeastOnce()
        from cdr in Try(Token(Char('.')).Then(Parser)).Optional()
        from _close in Char(')')
        select cdr.HasValue
            ? new SchemePair(
                cars.Count() == 1
                    ? cars.Single()
                    : SchemePair.FromEnumerable(cars),
                cdr.Value)
            : SchemePair.FromEnumerable(cars);

    private static Parser<char, SchemeDatum> ListParser => Token(
        OneOf(EmptyListParser, ListOrPairParser)
    ).Labelled("list");

    private static Parser<char, SchemeDatum> VectorParser => Token(
        from _open in Token(String("#("))
        from xs in Parser.Many()
        from _close in Char(')')
        select (SchemeDatum)new SchemeVector(xs.ToArray())
        ).Labelled("vector");

    private static Parser<char, SchemeDatum> AbbreviationParser =>
        from abbr in OneOf(
            Char('\'').Select(_ => SchemeSymbol.Known.Quote),
            Char('`').Select(_ => SchemeSymbol.Known.QuasiQuote),
            Char(',').Select(_ => SchemeSymbol.Known.Unquote),
            String(",@").Select(_ => SchemeSymbol.Known.UnquoteSplicing)
        )
        from v in Parser
        select (SchemeDatum)new SchemePair(abbr, new SchemePair(v, SchemeEmptyList.Value));

    #endregion
}