namespace Assemble.Scheme;

public sealed class SchemeNumber : SchemeDatum, Wraps<SchemeNumber, decimal>
{
    public SchemeNumber(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; init; }

    public override string Name => "number";

    public static SchemeNumber Wrap(decimal value)
    {
        return new SchemeNumber(value);
    }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeNumber b && b.Value == Value;
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }

    public decimal Unwrap()
    {
        return Value;
    }

    public override string Write()
    {
        return Value.ToString();
    }
}
