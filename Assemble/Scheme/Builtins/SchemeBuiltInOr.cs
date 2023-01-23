namespace Assemble.Scheme.Builtins;

public sealed class SchemeBuiltinOr : SchemeBuiltin
{
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