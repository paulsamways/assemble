namespace Assemble.Scheme.Builtins;

[SchemeBuiltin]
public sealed class SchemeBuiltinIf : SchemeBuiltin
{
    public override string Name => "if";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var p = arguments.To<SchemePair>();
        var test = SchemeBoolean.FromObject(p.Car.Evaluate(e));

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
