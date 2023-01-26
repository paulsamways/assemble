namespace Assemble.Scheme.Builtins;

[SchemeBuiltin]
public sealed class SchemeBuiltinBegin : SchemeBuiltin
{
    public override string Name => "begin";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        return arguments
            .To<SchemePair>()
            .AsEnumerable()
            .Select(x => x.Evaluate(e))
            .Last();
    }
}
