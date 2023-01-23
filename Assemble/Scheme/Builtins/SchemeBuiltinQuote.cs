namespace Assemble.Scheme.Builtins;

public sealed class SchemeBuiltinQuote : SchemeBuiltin
{
    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        return arguments.To<SchemePair>().Car;
    }
}