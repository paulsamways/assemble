namespace Assemble.Scheme;

public sealed class SchemeString : SchemeDatum
{
    public SchemeString(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeString b && b.Value.Equals(Value);
    }

    public override string Print()
    {
        // TODO: Escape specials
        return $"\"{Value}\"";
    }
}