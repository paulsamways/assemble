namespace Assemble.Scheme;

public class DatumParserTests
{
    [Fact]
    public void SchemeBoolean_ParseFromString()
    {
        Assert.True(DatumParser.Parse("#t").To<SchemeBoolean>().Value);
        Assert.True(DatumParser.Parse("#true").To<SchemeBoolean>().Value);

        Assert.False(DatumParser.Parse("#f").To<SchemeBoolean>().Value);
        Assert.False(DatumParser.Parse("#false").To<SchemeBoolean>().Value);
    }

    [Fact]
    public void SchemeNumber_ParseDecimal()
    {
        Assert.Equal(42, DatumParser.Parse("42").To<SchemeNumber>().Value);
        Assert.Equal(42, DatumParser.Parse("42.0").To<SchemeNumber>().Value);
        Assert.Equal(42, DatumParser.Parse("+42").To<SchemeNumber>().Value);
        Assert.Equal(42, DatumParser.Parse("+42.0").To<SchemeNumber>().Value);

        Assert.Equal(-42, DatumParser.Parse("-42").To<SchemeNumber>().Value);
        Assert.Equal(-42, DatumParser.Parse("-42.0").To<SchemeNumber>().Value);
        Assert.Equal(-42, DatumParser.Parse("-42").To<SchemeNumber>().Value);
        Assert.Equal(-42, DatumParser.Parse("-42.0").To<SchemeNumber>().Value);
    }

    [Fact]
    public void SchemeCharacter_ParseNames()
    {
        Assert.Equal(SchemeCharacter.Names.AlarmChar, DatumParser.Parse("#\\alarm").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.BackspaceChar, DatumParser.Parse("#\\backspace").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.DeleteChar, DatumParser.Parse("#\\delete").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.EscapeChar, DatumParser.Parse("#\\escape").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.NewLineChar, DatumParser.Parse("#\\newline").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.NullChar, DatumParser.Parse("#\\null").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.ReturnChar, DatumParser.Parse("#\\return").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.SpaceChar, DatumParser.Parse("#\\space").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.TabChar, DatumParser.Parse("#\\tab").To<SchemeCharacter>().Value);
    }

    [Fact]
    public void SchemeCharacter_ParseLetterOrDigit()
    {
        Assert.Equal('a', DatumParser.Parse("#\\a").To<SchemeCharacter>().Value);
        Assert.Equal('x', DatumParser.Parse("#\\x").To<SchemeCharacter>().Value);
        Assert.Equal('1', DatumParser.Parse("#\\1").To<SchemeCharacter>().Value);
    }

    [Fact]
    public void SchemeCharacter_ParseHex()
    {
        Assert.Equal('d', DatumParser.Parse("#\\x64").To<SchemeCharacter>().Value);
        Assert.Equal(SchemeCharacter.Names.AlarmChar, DatumParser.Parse("#\\x7").To<SchemeCharacter>().Value);
        Assert.Equal('\x5432', DatumParser.Parse("#\\x5432").To<SchemeCharacter>().Value);
    }

    [Fact]
    public void SchemeString_Parse()
    {
        Assert.Equal("Hello World", DatumParser.Parse("\"Hello World\"").To<SchemeString>().Value);
        Assert.Equal("", DatumParser.Parse("\"\"").To<SchemeString>().Value);
    }

    [Fact]
    public void SchemeSymbol_Parse()
    {
        Assert.Equal("apple", DatumParser.Parse("apple").To<SchemeSymbol>().Name);
    }

    [Fact]
    public void SchemeByteArray_Parse()
    {
        Assert.Equal(new byte[] { 1, 20, 222 }, DatumParser.Parse("#u8(1 20 222)").To<SchemeBytevector>().Value);
    }

    [Fact]
    public void SchemeEmptyList_Parse()
    {
        Assert.Equal(SchemeEmptyList.Value, DatumParser.Parse("()").To<SchemeEmptyList>());
    }

    [Fact]
    public void SchemePair_ParseList_List()
    {
        var p1 = DatumParser.Parse("(1 2 3)").To<SchemePair>();
        var p2 = p1.Cdr.To<SchemePair>();
        var p3 = p2.Cdr.To<SchemePair>();

        Assert.Equal(1, p1.Car.To<SchemeNumber>().Value);
        Assert.Equal(2, p2.Car.To<SchemeNumber>().Value);
        Assert.Equal(3, p3.Car.To<SchemeNumber>().Value);

        Assert.Equal(SchemeEmptyList.Value, p3.Cdr);
    }

    [Fact]
    public void SchemePair_ParseList_ImproperList()
    {
        var p1 = DatumParser.Parse("(1 2 . 3)").To<SchemePair>();
        var p1car = p1.Car.To<SchemePair>();
        var p1cdr = p1.Cdr.To<SchemeNumber>();

        var p2car = p1car.Car.To<SchemeNumber>();
        var p2cdr = p1car.Cdr.To<SchemePair>();

        var p3car = p2cdr.Car.To<SchemeNumber>();
        p2cdr.Cdr.To<SchemeEmptyList>();

        Assert.Equal(1, p2car.Value);
        Assert.Equal(2, p3car.Value);
        Assert.Equal(3, p1cdr.Value);
    }

    [Fact]
    public void SchemePair_ParseList_Pair()
    {
        var p1 = DatumParser.Parse("(1 . 2)").To<SchemePair>();
        var p1car = p1.Car.To<SchemeNumber>();
        var p1cdr = p1.Cdr.To<SchemeNumber>();

        Assert.Equal(1, p1car.Value);
        Assert.Equal(2, p1cdr.Value);
    }

    [Fact]
    public void SchemeVector_ParseNonEmpty()
    {
        var v = DatumParser.Parse("#(1 2 3)").To<SchemeVector>();
        Assert.Equal(1, v.Values[0].To<SchemeNumber>().Value);
        Assert.Equal(2, v.Values[1].To<SchemeNumber>().Value);
        Assert.Equal(3, v.Values[2].To<SchemeNumber>().Value);
    }

    [Fact]
    public void SchemeVector_ParseEmpty()
    {
        var v = DatumParser.Parse("#()").To<SchemeVector>();
        Assert.Empty(v.Values);
    }

    [Fact]
    public void SchemeAbbreviation_ParseQuote()
    {
        var v = DatumParser.Parse("'()").To<SchemePair>();

        Assert.Equal(v.Car, SchemeSymbol.Known.Quote);
        Assert.Equal(v.Cdr.To<SchemePair>().Car, SchemeEmptyList.Value);
    }

    [Fact]
    public void SchemeAbbreviation_ParseQuasiQuote()
    {
        var v = DatumParser.Parse("`()").To<SchemePair>();

        Assert.Equal(v.Car, SchemeSymbol.Known.QuasiQuote);
        Assert.Equal(v.Cdr.To<SchemePair>().Car, SchemeEmptyList.Value);
    }
}