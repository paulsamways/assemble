namespace Assemble.Scheme.Builtins;

public sealed class SchemeBuiltinProcedure : SchemeBuiltin
{
    public SchemeBuiltinProcedure(Func<Environment, SchemeObject[], SchemeObject> func)
    {
        Func = func;
    }

    public Func<Environment, SchemeObject[], SchemeObject> Func { get; }

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        return Func(e, arguments
            .To<SchemePair>()
            .AsEnumerable()
            .Select(x => x.Evaluate(e))
            .ToArray());
    }

    public static Func<SchemeObject, SchemeObject> BinaryOp(Func<decimal, decimal, decimal> func)
    {
        return (x) =>
        {
            var pair = x.To<SchemePair>();
            var a = pair.Car.To<SchemeNumber>();
            pair = pair.Cdr.To<SchemePair>();
            var b = pair.Car.To<SchemeNumber>();
            pair.Cdr.To<SchemeEmptyList>();

            return new SchemeNumber(func(a.Value, b.Value));
        };
    }
}