namespace Assemble.Scheme;

public sealed class SchemeBoolean : SchemeDatum, Wraps<SchemeBoolean, bool>
{
    private SchemeBoolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; init; }

    public override string Name => "boolean";

    public static SchemeBoolean FromBoolean(bool value)
    {
        return value ? True : False;
    }

    public override string Write()
    {
        return Value ? "#t" : "#f";
    }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeBoolean b && b.Value == Value;
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }

    public bool Unwrap()
    {
        return Value;
    }

    public static SchemeBoolean Wrap(bool value)
    {
        return FromBoolean(value);
    }

    public static readonly SchemeBoolean True = new(true);
    public static readonly SchemeBoolean False = new(false);
}