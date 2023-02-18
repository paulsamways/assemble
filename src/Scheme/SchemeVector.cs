namespace Scheme;

public sealed class SchemeVector : SchemeDatum
{
    public SchemeVector(SchemeObject[] values)
    {
        Values = values;
    }

    public SchemeObject[] Values { get; private set; }

    public override string Name
        => "vector";


    public override string ToString()
        => $"#({string.Join(" ", Values.Select(x => x.ToString()))})";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeVector p && p.Values.SequenceEqual(Values);

    public override int GetHashCode()
        => Values.GetHashCode();

    public override SchemeObject Visit(SchemeObjectVisitor v)
        => v.OnSchemeVector(this, Values.Select(x => x.Visit(v)).ToArray());
}