namespace Assemble.Scheme;

public sealed class SchemeSymbol : SchemeDatum
{
    private static readonly Dictionary<string, SchemeSymbol> _interns = new();

    private SchemeSymbol(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public override string Name => "symbol";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeSymbol b && b.Value.Equals(Value);
    public override bool Same(SchemeObject other)
        => Equals(other);

    public override string Write()
    {
        return Value;
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return e.Get(this) ?? throw new Exception($"unbound symbol: {Value}");
    }

    public static SchemeSymbol FromString(string value)
    {
        if (!_interns.TryGetValue(value, out var symbol))
            symbol = _interns[value] = new SchemeSymbol(value);

        return symbol;
    }

    public static class Known
    {
        public static readonly SchemeSymbol Quote = FromString("quote");
        public static readonly SchemeSymbol QuasiQuote = FromString("quasiquote");
        public static readonly SchemeSymbol Unquote = FromString("unquote");
        public static readonly SchemeSymbol UnquoteSplicing = FromString("unquote-splicing");

        public static readonly SchemeSymbol Set = FromString("set!");
        public static readonly SchemeSymbol If = FromString("if");
        public static readonly SchemeSymbol And = FromString("and");
        public static readonly SchemeSymbol Or = FromString("or");
        public static readonly SchemeSymbol Not = FromString("not");
        public static readonly SchemeSymbol Lambda = FromString("lambda");

        public static readonly SchemeSymbol Null_ = FromString("null?");
        public static readonly SchemeSymbol List_ = FromString("list?");
        public static readonly SchemeSymbol Pair_ = FromString("pair?");
        public static readonly SchemeSymbol Cons = FromString("cons");
        public static readonly SchemeSymbol Car = FromString("car");
        public static readonly SchemeSymbol Cdr = FromString("cdr");
        public static readonly SchemeSymbol SetCar = FromString("set-car!");
        public static readonly SchemeSymbol SetCdr = FromString("set-cdr!");

        public static readonly SchemeSymbol Apply = FromString("apply");
        public static readonly SchemeSymbol Map = FromString("map");
    }
}
