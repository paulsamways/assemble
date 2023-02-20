using System.Text;
using Pidgin;
using Scheme.Expander;
using static Pidgin.Parser;

namespace Scheme.Compiler;

public class Parser
{
    private readonly bool _asSyntaxObject;
    private string _source = string.Empty;

    public Parser(bool asSyntaxObject = false)
    {
        _asSyntaxObject = asSyntaxObject;
    }

    public SchemeObject Parse(string input)
    {
        _source = input;

        return (
            from _ in SkipWhitespaces
            from e in DatumParser
            from _end in Parser<char>.End
            select e
        ).ParseOrThrow(input);
    }

    private static Parser<char, T> Token<T>(Parser<char, T> p)
        => Try(p);

    private Parser<char, SchemeObject> SyntaxObject(Parser<char, SchemeObject> p) =>
        _asSyntaxObject
            ? from pos in Parser<char>.CurrentPos
              from start in Parser<char>.CurrentOffset
              from x in Parser<char>.CurrentSourcePosDelta
              from datum in p
              from end in Parser<char>.CurrentOffset
              select (SchemeObject)new SchemeSyntaxObject((SchemeDatum)datum, new SourceInfo(_source, pos.Line, pos.Col, start, end - start))
            : p;

    /// <summary>
    /// Parser for the datum production rule:
    /// <code>
    /// 〈datum〉 −→ 〈simple datum〉| 〈compound datum〉| 〈label〉 = 〈datum〉 | 〈label〉 #
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> DatumParser =>
        SyntaxObject(OneOf(SimpleDatum, CompoundDatum))
            .Before(SkipWhitespaces);

    #region Simple Datums

    /// <summary>
    /// Parser for the simple datum production rule:
    /// <code>
    /// 〈simple datum〉 −→ 〈boolean〉
    ///                     | 〈number〉
    ///                     | 〈character〉
    ///                     | 〈string〉
    ///                     | 〈symbol〉
    ///                     | 〈bytevector〉
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> SimpleDatum =>
        OneOf(BooleanParser, NumberParser, CharacterParser, StringParser, SymbolParser, ByteVectorParser);

    /// <summary>
    /// Parser for the boolean production rule:
    /// <code>
    /// 〈boolean〉 −→ #t | #f | #true | #false
    /// </code>
    /// </summary>
    private static Parser<char, SchemeObject> BooleanParser => Token(
        from _ in Char('#')
        from v in OneOf(
            Char('t').Before(Try(String("rue")).Optional()),
            Char('f').Before(Try(String("alse")).Optional()))
        select (SchemeObject)SchemeBoolean.FromBoolean(v == 't')
    ).Labelled("boolean");

    /// <summary>
    /// Parser for the number production rule:
    /// <code>
    /// 〈number〉 −→ 〈num 2〉 | 〈num 8〉| 〈num 10〉 | 〈num 16〉
    /// </code>
    /// </summary>
    private static Parser<char, SchemeObject> NumberParser => Token(
        from sign in Try(Char('-').Or(Char('+')).Optional())
        from w in Digit.AtLeastOnceString()
        from d in Try(Char('.').Then(Digit.AtLeastOnceString())).Optional()
        select (SchemeObject)new SchemeNumber(ParseDecimal(sign, w, d))
    ).Labelled("number");

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

    /// <summary>
    /// Parser for the character production rule:
    /// <code>
    /// 〈character〉 −→ #\ 〈any character〉| #\ 〈character name〉| #\x〈hex scalar value〉
    /// </code>
    /// </summary>
    private static Parser<char, SchemeObject> CharacterParser => Token(
        from _ in String("#\\")
        from v in OneOf(Try(CharacterNameParser), Try(CharacterHexParser), LetterOrDigit)
        select (SchemeObject)new SchemeCharacter(v)
    ).Labelled("character");

    /// <summary>
    /// Parser for the character name production rule:
    /// <code>
    /// 〈character name〉 −→ alarm | backspace | delete | escape | newline | null | return | space | tab
    /// </code>
    /// </summary>
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

    /// <summary>
    /// Parser for the string production rule:
    /// <code>
    /// 〈string〉 −→ " 〈string element〉* "
    /// </code>
    /// </summary>
    private static Parser<char, SchemeObject> StringParser => Token(
        from v in AnyCharExcept('"').ManyString().Between(Char('"'))
        select (SchemeObject)new SchemeString(v)
    ).Labelled("string");

    /// <summary>
    /// Parser for the identifier production rule:
    /// <code>
    /// 〈identifier〉 −→ 〈initial〉 〈subsequent〉*
    ///                 | 〈vertical line〉 〈symbol element〉* 〈vertical line〉
    ///                 | 〈peculiar identifier〉
    /// </code>
    /// </summary>
    private Parser<char, string> IdentifierParser =>
        Try(PeculiarIdentifier).Or(
            from x in InitialParser
            from xs in SubsequentParser.ManyString()
            select x + xs
        );

    /// <summary>
    /// Parser for the initial production rule:
    /// <code>
    /// 〈initial〉 −→ 〈letter〉 | 〈special initial〉
    /// </code>
    /// </summary>
    private Parser<char, char> InitialParser =>
        LetterParser.Or(SpecialInitialParser);

    /// <summary>
    /// Parser for the letter production rule:
    /// <code>
    /// 〈letter〉 −→ a | b | c | ... | z | A | B | C | ... | Z
    /// </code>
    /// </summary>
    private Parser<char, char> LetterParser =>
        Parser<char>.Token(char.IsAsciiLetter);

    /// <summary>
    /// Parser for the special initial production rule:
    /// <code>
    /// 〈special initial〉 −→ ! | $ | % | & | * | / | : | < | = | > | ? | ^ | _ | ~
    /// </code>
    /// </summary>
    private Parser<char, char> SpecialInitialParser =>
        OneOf("!$%&*/:<=>?^_~");

    /// <summary>
    /// Parser for the subsequent production rule:
    /// <code>
    /// 〈subsequent〉 −→ 〈initial〉 | 〈digit〉 | 〈special subsequent
    /// </code>
    /// </summary>
    private Parser<char, char> SubsequentParser =>
        InitialParser.Or(Digit).Or(SpecialSubsequentParser);

    /// <summary>
    /// Parser for the special subsequent production rule:
    /// <code>
    /// 〈special subsequent〉 −→ 〈explicit sign〉 | . | @
    /// </code>
    /// </summary>
    private Parser<char, char> SpecialSubsequentParser =>
        ExplicitSignParser.Or(OneOf("@."));

    /// <summary>
    /// Parser for the explicit sign production rule:
    /// <code>
    /// 〈explicit sign〉 −→ + | -
    /// </code>
    /// </summary>
    private Parser<char, char> ExplicitSignParser =>
        OneOf("+-");

    /// <summary>
    /// Parser for the peculiar identifier production rule:
    /// <code>
    /// 〈peculiar identifier〉 −→ 〈explicit sign〉
    ///                         | 〈explicit sign〉 〈sign subsequent〉 〈subsequent〉*
    ///                         | 〈explicit sign〉 . 〈dot subsequent〉 〈subsequent〉*
    ///                         | . 〈dot subsequent〉 〈subsequent〉
    /// </code>
    /// </summary>
    private Parser<char, string> PeculiarIdentifier =>
        from x in ExplicitSignParser
        select x.ToString();

    /// <summary>
    /// Parser for the symbol production rule:
    /// <code>
    /// 〈symbol〉 −→ 〈identifier〉
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> SymbolParser => Token(
        from n in IdentifierParser
        select (SchemeObject)SchemeSymbol.FromString(n)
    ).Labelled("symbol");

    /// <summary>
    /// Parser for the bytevector production rule:
    /// <code>
    /// 〈bytevector〉 −→ #u8(〈byte〉*)
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> ByteVectorParser => Token(
        from _u8 in String("#u8")
        from _o in Char('(').Then(SkipWhitespaces)
        from xs in ByteParser.SeparatedAndOptionallyTerminated(Whitespaces)
        from _c in Char(')')
        select (SchemeObject)new SchemeBytevector(xs.ToArray())
    ).Labelled("bytevector");

    /// <summary>
    /// Parser for the byte production rule:
    /// <code>
    /// 〈byte〉 −→ 〈any exact integer between 0 and 255〉
    /// </code>
    /// </summary>
    private static Parser<char, byte> ByteParser =>
        Digit.AtLeastOnceString().Select(x => byte.Parse(x));

    #endregion

    #region Compound Datums

    /// <summary>
    /// Parser for the compound datum production rule:
    /// <code>
    /// 〈compound datum〉 −→ 〈list〉 | 〈vector〉 | 〈abbreviation〉
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> CompoundDatum =>
        OneOf(ListParser, VectorParser, AbbreviationParser);

    /// <summary>
    /// Parser for the list production rule:
    /// <code>
    /// 〈list〉 −→ (〈datum〉*) | (〈datum〉+ . 〈datum〉)
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> ListParser => Token(
        OneOf(EmptyListParser, ListOrPairParser)
    ).Labelled("list");

    private Parser<char, SchemeObject> EmptyListParser =>
        Token(Whitespaces.Between(Char('('), Char(')')))
            .Select(_ => (SchemeObject)SchemeEmptyList.Value);

    private Parser<char, SchemeObject> ListOrPairParser => Token(
        from _open in Char('(')
        from _ in SkipWhitespaces
        from cars in DatumParser.AtLeastOnce()
        from cdr in Try(Char('.').Then(SkipWhitespaces).Then(DatumParser)).Optional()
        from _close in Char(')')
        select cdr.HasValue
            ? new SchemePair(
                cars.Count() == 1
                    ? cars.Single()
                    : SchemePair.FromEnumerable(cars),
                cdr.Value)
            : SchemePair.FromEnumerable(cars)
    );

    /// <summary>
    /// Parser for the vector production rule:
    /// <code>
    /// 〈vector〉 −→ #(〈datum〉*)
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> VectorParser => Token(
        from _open in String("#(")
        from _ in SkipWhitespaces
        from xs in DatumParser.Many()
        from _close in Char(')')
        select (SchemeObject)new SchemeVector(xs.ToArray())
        ).Labelled("vector");

    /// <summary>
    /// Parser for the abbreviation production rule:
    /// <code>
    /// 〈abbreviation〉 −→ 〈abbrev prefix〉 〈datum〉
    /// </code>
    /// </summary>
    private Parser<char, SchemeObject> AbbreviationParser =>
        from abbr in SyntaxObject(OneOf(
            Char('\'').Select(_ => (SchemeObject)SchemeSymbol.Known.Quote),
            Char('`').Select(_ => (SchemeObject)SchemeSymbol.Known.QuasiQuote),
            Char(',').Select(_ => (SchemeObject)SchemeSymbol.Known.Unquote),
            String(",@").Select(_ => (SchemeObject)SchemeSymbol.Known.UnquoteSplicing)
        ))
        from v in DatumParser
        select (SchemeObject)new SchemePair(abbr, new SchemePair(v, SchemeEmptyList.Value));

    #endregion
}