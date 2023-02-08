using Scheme.Compiler.Instructions;

namespace Scheme;

public sealed class SchemeEmptyList : SchemeDatum
{
    private SchemeEmptyList()
    {
    }

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeEmptyList;

    public override bool Same(SchemeObject other)
        => Equals(other);

    public override string Name => "null";

    public override string ToString() => "()";

    public static readonly SchemeEmptyList Value = new();
}