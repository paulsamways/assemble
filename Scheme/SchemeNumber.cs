namespace Scheme;

public sealed class SchemeNumber : SchemeDatum, Wraps<SchemeNumber, decimal>
{
    public SchemeNumber(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; init; }

    public override string Name => "number";

    public override bool Same(SchemeObject other)
        => Equals(other);

    public static SchemeNumber Wrap(decimal value)
        => new(value);

    public decimal Unwrap() => Value;

    public override string ToString() => Value.ToString();

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeNumber b && b.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();
}
