namespace Assemble.Scheme.Builtins;

[SchemeBuiltin]
public sealed class SchemeBuiltinLambda : SchemeBuiltin
{
    public override string Name => "lambda";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var p = arguments.To<SchemePair>();

        var parameters = p.Car.To<SchemePair>().AsEnumerable().Select(x => x.To<SchemeSymbol>().Value);
        var body = p.Cdr.To<SchemePair>().Car;

        return new SchemeProcedure(e, parameters.ToArray(), new SchemeObject[] { body });
    }
}