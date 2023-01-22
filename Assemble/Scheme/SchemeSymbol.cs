namespace Assemble.Scheme;

public sealed class SchemeSymbol : SchemeDatum
{
    private static readonly Dictionary<string, SchemeSymbol> _interns = new();

    private SchemeSymbol(string name)
    {
        Name = name;
    }

    public string Name { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeSymbol b && b.Name.Equals(Name);
    }

    public override string Print()
    {
        return Name;
    }

    public static SchemeSymbol FromString(string name)
    {
        if (!_interns.TryGetValue(name, out var symbol))
            symbol = _interns[name] = new SchemeSymbol(name);

        return symbol;
    }

    public static class Known
    {
        public static readonly SchemeSymbol Quote = FromString("quote");
        public static readonly SchemeSymbol QuasiQuote = FromString("quasiquote");
        public static readonly SchemeSymbol Unquote = FromString("unquote");
        public static readonly SchemeSymbol UnquoteSplicing = FromString("unquote-splicing");
    }
}
