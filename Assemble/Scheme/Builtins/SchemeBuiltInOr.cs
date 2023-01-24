namespace Assemble.Scheme.Builtins;

[SchemeBuiltin]
public sealed class SchemeBuiltinOr : SchemeBuiltin
{
    public override string Name => "or";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        foreach (var argument in arguments.To<SchemePair>().AsEnumerable())
        {
            if (argument.Evaluate(e).To<SchemeBoolean>().Value)
            {
                return SchemeBoolean.True;
            }
        }

        return SchemeBoolean.False;
    }
}