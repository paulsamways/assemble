namespace Scheme;

public sealed class SchemeVector : SchemeDatum
{
    public SchemeVector(SchemeObject[] values)
    {
        Values = values;
    }

    public SchemeObject[] Values { get; init; }

    public override string Name
        => "vector";

    public override string ToString()
        => $"#({string.Join(" ", Values.Select(x => x.ToString()))})";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeVector p && p.Values.SequenceEqual(Values);

    public override int GetHashCode()
        => Values.GetHashCode();
}