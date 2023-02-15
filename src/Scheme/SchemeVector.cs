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

    public override SchemeSyntaxObject ToSyntaxObject()
        => new(
            new SchemeVector(
                Values
                    .Select(x => x is SchemeDatum d ? d.ToSyntaxObject() : x)
                    .ToArray()
            )
        );

    public override SchemeDatum ToDatum()
        => new SchemeVector(Values.Select(x => x.ToDatum()).ToArray());

    public override string ToString()
        => $"#({string.Join(" ", Values.Select(x => x.ToString()))})";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeVector p && p.Values.SequenceEqual(Values);

    public override int GetHashCode()
        => Values.GetHashCode();
}