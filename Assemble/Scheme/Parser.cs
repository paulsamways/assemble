using System.Text;
using Pidgin;
using static Pidgin.Parser;

namespace Assemble.Scheme;

public static class Parser
{
    public static SchemeObject Parse(string input)
    {
        return (
            from _ in SkipWhitespaces
            from e in DatumParser
            from _end in Parser<char>.End
            select e
        ).ParseOrThrow(input);
    }

    private static Parser<char, T> Token<T>(Parser<char, T> p)
        => Try(p).Before(SkipWhitespaces);

    private static Parser<char, SchemeObject> DatumParser =>
        OneOf(SimpleDatum, CompoundDatum);

    #region Simple Datums

    private static Parser<char, SchemeObject> SimpleDatum =>
        OneOf(BooleanParser, NumberParser, CharacterParser, StringParser, SymbolParser, ByteVectorParser);

    private static Parser<char, SchemeObject> BooleanParser => Token(
        from _ in Char('#')
        from v in OneOf(
            Char('t').Before(Try(String("rue")).Optional()),
            Char('f').Before(Try(String("alse")).Optional()))
        select (SchemeObject)SchemeBoolean.FromBoolean(v == 't')
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

    private static Parser<char, SchemeObject> NumberParser => Token(
        from sign in Try(Char('-').Or(Char('+')).Optional())
        from w in Digit.AtLeastOnceString()
        from d in Try(Char('.').Then(Digit.AtLeastOnceString())).Optional()
        select (SchemeObject)new SchemeNumber(ParseDecimal(sign, w, d))
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

    private static Parser<char, SchemeObject> CharacterParser => Token(
        from _ in String("#\\")
        from v in OneOf(Try(CharacterNameParser), Try(CharacterHexParser), LetterOrDigit)
        select (SchemeObject)new SchemeCharacter(v)
    ).Labelled("character");

    private static Parser<char, SchemeObject> StringParser => Token(
        from v in AnyCharExcept('"').ManyString().Between(Char('"'))
        select (SchemeObject)new SchemeString(v)
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

    private static Parser<char, SchemeObject> SymbolParser => Token(
        from n in IdentifierParser
        select (SchemeObject)SchemeSymbol.FromString(n)
    ).Labelled("symbol");

    private static Parser<char, SchemeObject> ByteVectorParser => Token(
        from xs in
            String("#u8").Then(
                ByteParser
                    .Separated(Whitespaces)
                .Between(Char('('), Char(')')))
        select (SchemeObject)new SchemeBytevector(xs.ToArray())
    ).Labelled("bytevector");

    private static Parser<char, byte> ByteParser =>
        Digit.AtLeastOnceString().Select(x => byte.Parse(x));

    #endregion

    #region Compound Datums

    private static Parser<char, SchemeObject> CompoundDatum =>
        OneOf(ListParser, VectorParser, AbbreviationParser);

    private static Parser<char, SchemeObject> EmptyListParser =>
        Token(Whitespaces.Between(Char('('), Char(')')))
            .Select(_ => (SchemeObject)SchemeEmptyList.Value);

    private static Parser<char, SchemeObject> ListOrPairParser =>
        from _open in Token(Char('('))
        from cars in DatumParser.AtLeastOnce()
        from cdr in Try(Token(Char('.')).Then(DatumParser)).Optional()
        from _close in Char(')')
        select cdr.HasValue
            ? new SchemePair(
                cars.Count() == 1
                    ? cars.Single()
                    : SchemePair.FromEnumerable(cars),
                cdr.Value)
            : SchemePair.FromEnumerable(cars);

    private static Parser<char, SchemeObject> ListParser => Token(
        OneOf(EmptyListParser, ListOrPairParser)
    ).Labelled("list");

    private static Parser<char, SchemeObject> VectorParser => Token(
        from _open in Token(String("#("))
        from xs in DatumParser.Many()
        from _close in Char(')')
        select (SchemeObject)new SchemeVector(xs.ToArray())
        ).Labelled("vector");

    private static Parser<char, SchemeObject> AbbreviationParser =>
        from abbr in OneOf(
            Char('\'').Select(_ => SchemeSymbol.Known.Quote),
            Char('`').Select(_ => SchemeSymbol.Known.QuasiQuote),
            Char(',').Select(_ => SchemeSymbol.Known.Unquote),
            String(",@").Select(_ => SchemeSymbol.Known.UnquoteSplicing)
        )
        from v in DatumParser
        select (SchemeObject)new SchemePair(abbr, new SchemePair(v, SchemeEmptyList.Value));

    #endregion
}