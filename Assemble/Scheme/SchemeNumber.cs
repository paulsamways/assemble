namespace Assemble.Scheme;

public sealed class SchemeNumber : SchemeDatum
{
    public SchemeNumber(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeNumber b && b.Value == Value;
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }

    public override string Write()
    {
        return Value.ToString();
    }
}
