using Assemble.Scheme.Builtins;

namespace Assemble.Scheme;

public class Environment
{
    private readonly Environment? _parent;

    private readonly Dictionary<string, SchemeObject> _objects;

    public Environment(Environment? parent = null)
    {
        _objects = new();
        _parent = parent;
    }

    public SchemeObject? Get(SchemeSymbol symbol)
    {
        if (_objects.TryGetValue(symbol.Name, out var o))
            return o;

        if (_parent is not null)
            return _parent.Get(symbol);

        return null;
    }
    public void Set(SchemeSymbol symbol, SchemeObject o)
    {
        _objects[symbol.Name] = o;
    }

    public static Environment Default()
    {
        var environment = new Environment();
        environment.Set(SchemeSymbol.Known.Quote, new SchemeBuiltinQuote());
        environment.Set(SchemeSymbol.Known.Set, new SchemeBuiltinSet());
        environment.Set(SchemeSymbol.Known.If, new SchemeBuiltinConditional());
        environment.Set(SchemeSymbol.Known.Not, new SchemeBuiltinProcedure((_, xs) => SchemeBoolean.FromBoolean(!xs[0].To<SchemeBoolean>().Value)));
        environment.Set(SchemeSymbol.Known.And, new SchemeBuiltinAnd());
        environment.Set(SchemeSymbol.Known.Or, new SchemeBuiltinOr());
        environment.Set(SchemeSymbol.Known.Lambda, new SchemeBuiltinLambda());

        environment.Set(SchemeSymbol.FromString("add"), new SchemeBuiltinBinaryNumericFunc((a, b) => a + b));
        environment.Set(SchemeSymbol.FromString("inc"), new SchemeProcedure(
            environment,
            new string[] { "a" },
            new SchemeObject[] { Parser.Parse("(add a 1)") }
        ));

        environment.Set(SchemeSymbol.Known.Null_, new SchemeBuiltinProcedure((_, xs) => SchemeBoolean.FromBoolean(xs[0] is SchemeEmptyList)));
        environment.Set(SchemeSymbol.Known.List_, new SchemeBuiltinProcedure((_, xs) => SchemeBoolean.FromBoolean(xs[0] is SchemePair p && p.IsList)));
        environment.Set(SchemeSymbol.Known.Pair_, new SchemeBuiltinProcedure((_, xs) => SchemeBoolean.FromBoolean(xs[0] is SchemePair)));
        environment.Set(SchemeSymbol.Known.Cons, new SchemeBuiltinProcedure((_, xs) => new SchemePair(xs[0], xs[1])));
        environment.Set(SchemeSymbol.Known.Car, new SchemeBuiltinProcedure((_, xs) => xs[0].To<SchemePair>().Car));
        environment.Set(SchemeSymbol.Known.Cdr, new SchemeBuiltinProcedure((_, xs) => xs[0].To<SchemePair>().Cdr));
        environment.Set(SchemeSymbol.Known.SetCar, new SchemeBuiltinProcedure((_, xs) =>
        {
            xs[0].To<SchemePair>().Car = xs[1];
            return SchemeUndefined.Value;
        }));
        environment.Set(SchemeSymbol.Known.SetCdr, new SchemeBuiltinProcedure((_, xs) =>
        {
            xs[0].To<SchemePair>().Cdr = xs[1];
            return SchemeUndefined.Value;
        }));

        environment.Set(SchemeSymbol.Known.Apply, new SchemeBuiltinProcedure((e, xs) => xs[0].To<SchemeProcedure>().Call(e, SchemePair.FromEnumerable(xs[1..]))));

        return environment;
    }
}