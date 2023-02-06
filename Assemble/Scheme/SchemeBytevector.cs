namespace Assemble.Scheme;

public sealed class SchemeBytevector : SchemeDatum
{
    public SchemeBytevector(byte[] value)
    {
        Value = value;
    }

    public byte[] Value { get; init; }

    public override string Name => "bytevector";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeBytevector b && b.Value.SequenceEqual(Value);

    public override string Write()
        => $"u8({string.Join(" ", Value)})";
}