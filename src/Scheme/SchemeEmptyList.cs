namespace Scheme;

public sealed class SchemeEmptyList : SchemeDatum
{
    private SchemeEmptyList()
    {
    }

    public override bool Same(SchemeObject other)
        => Equals(other);

    public override string Name => "null";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeEmptyList;

    public override string ToString() => "()";

    public override int GetHashCode() => 17;

    public static readonly SchemeEmptyList Value = new();

    public override SchemeObject Visit(SchemeObjectVisitor v)
        => v.OnSchemeEmptyList(this);
}