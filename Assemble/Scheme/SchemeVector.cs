namespace Assemble.Scheme;

public sealed class SchemeVector : SchemeDatum
{
    public SchemeVector(SchemeObject[] values)
    {
        Values = values;
    }

    public SchemeObject[] Values { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeVector p && p.Values.SequenceEqual(Values);
    }

    public override string Write()
    {
        return $"#({string.Join(" ", Values.Select(x => x.Write()))})";
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }
}