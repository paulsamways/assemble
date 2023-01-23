namespace Assemble.Scheme.Builtins;

public sealed class SchemeBuiltinBinaryNumericFunc : SchemeBuiltin
{
    public SchemeBuiltinBinaryNumericFunc(Func<decimal, decimal, decimal> func)
    {
        Func = func;
    }

    public Func<decimal, decimal, decimal> Func { get; }

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var result = arguments
            .To<SchemePair>()
            .AsEnumerable()
            .Select(x => x.Evaluate(e).To<SchemeNumber>().Value)
            .Aggregate(Func);

        return new SchemeNumber(result);
    }
}