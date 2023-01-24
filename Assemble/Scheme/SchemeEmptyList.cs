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

    public override string Name => "null";

    public override string Write()
    {
        return "()";
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }

    public static readonly SchemeEmptyList Value = new();
}