namespace Assemble.Scheme;

public sealed class SchemeUndefined : SchemeObject
{
    private SchemeUndefined()
    {
    }

    public static readonly SchemeUndefined Value = new();

    public override string Name => "undefined";

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }
}