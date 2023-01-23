namespace Assemble.Scheme.Builtins;

public sealed class SchemeBuiltinConditional : SchemeBuiltin
{
    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var p = arguments.To<SchemePair>();
        var test = p.Car.Evaluate(e).To<SchemeBoolean>();

        p = p.Cdr.To<SchemePair>();
        if (test.Value)
        {
            return p.Car.Evaluate(e);
        }
        else if (p.Cdr is SchemePair p2)
        {
            return p2.Car.Evaluate(e);
        }
        else
        {
            return SchemeUndefined.Value;
        }
    }
}
