namespace Assemble.Scheme;

public sealed class SchemeVector : SchemeDatum
{
    public SchemeVector(SchemeObject[] values)
    {
        Values = values;
    }

    public SchemeObject[] Values { get; init; }

    public override string Name
        => "vector";

    public override string Write()
        => $"#({string.Join(" ", Values.Select(x => x.Write()))})";

    public override SchemeObject Evaluate(Environment e)
        => this;

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeVector p && p.Values.SequenceEqual(Values);
}