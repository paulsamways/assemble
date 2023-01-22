namespace Assemble.Scheme;

public sealed class SchemeVector : SchemeDatum
{
    public SchemeVector(SchemeDatum[] values)
    {
        Values = values;
    }

    public SchemeDatum[] Values { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeVector p && p.Values.SequenceEqual(Values);
    }

    public override string Print()
    {
        return $"#({string.Join(" ", Values.Select(x => x.Print()))})";
    }
}