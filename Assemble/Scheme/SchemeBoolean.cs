namespace Assemble.Scheme;

public sealed class SchemeBoolean : SchemeDatum
{
    private SchemeBoolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; init; }

    public static SchemeBoolean FromBoolean(bool value)
    {
        return value ? True : False;
    }

    public override string Print()
    {
        return Value ? "#t" : "#f";
    }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeBoolean b && b.Value == Value;
    }

    private static readonly SchemeBoolean True = new(true);
    private static readonly SchemeBoolean False = new(false);
}