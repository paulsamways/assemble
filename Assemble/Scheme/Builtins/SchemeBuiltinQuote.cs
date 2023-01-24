namespace Assemble.Scheme.Builtins;

[SchemeBuiltin]
public sealed class SchemeBuiltinQuote : SchemeBuiltin
{
    public override string Name => "quote";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        return arguments.To<SchemePair>().Car;
    }
}