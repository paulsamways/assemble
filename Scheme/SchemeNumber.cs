using Scheme.Compiler.Instructions;

namespace Scheme;

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

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeNumber b && b.Value == Value;

    public override bool Same(SchemeObject other)
        => Equals(other);

    public decimal Unwrap()
    {
        return Value;
    }

    public override string Write()
    {
        return Value.ToString();
    }
}
