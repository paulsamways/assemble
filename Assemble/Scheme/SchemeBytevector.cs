namespace Assemble.Scheme;

public sealed class SchemeBytevector : SchemeDatum
{
    public SchemeBytevector(byte[] value)
    {
        Value = value;
    }

    public byte[] Value { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeBytevector b && b.Value.SequenceEqual(Value);
    }

    public override string Print()
    {
        return $"u8({string.Join(" ", Value)})";
    }
}