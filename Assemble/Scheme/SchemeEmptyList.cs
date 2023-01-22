namespace Assemble.Scheme;

public sealed class SchemeEmptyList : SchemeDatum
{
    private SchemeEmptyList()
    {
    }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemeEmptyList;
    }

    public override string Print()
    {
        return "()";
    }

    public static readonly SchemeEmptyList Value = new();
}