namespace Assemble.Scheme.Builtins;

public sealed class SchemeBuiltinSet : SchemeBuiltin
{
    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var p = arguments.To<SchemePair>();
        var symbol = p.Car.To<SchemeSymbol>();
        var value = p.Cdr.To<SchemePair>().Car.Evaluate(e);

        e.Set(symbol, value);

        return value;
    }
}