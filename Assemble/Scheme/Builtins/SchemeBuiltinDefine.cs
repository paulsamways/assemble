namespace Assemble.Scheme.Builtins;

[SchemeBuiltin]
public sealed class SchemeBuiltinDefine : SchemeBuiltin
{
    public override string Name => "define";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var p = arguments.To<SchemePair>();

        if (p.Car is SchemeSymbol s)
        {
            p = p.Cdr.To<SchemePair>();
            p.Cdr.To<SchemeEmptyList>();

            var v = p.Car.Evaluate(e);

            e.Set(s, v);
        }

        return SchemeUndefined.Value;
    }
}
