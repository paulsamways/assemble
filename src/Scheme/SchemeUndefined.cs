namespace Scheme;

public sealed class SchemeUndefined : SchemeObject
{
    private SchemeUndefined()
    {
    }

    public static readonly SchemeUndefined Value = new();

    public override string Name => "undefined";

    public override bool Equals(SchemeObject? other) => false;

    public override bool Same(SchemeObject other) => false;

    public override int GetHashCode()
        => 61;
}