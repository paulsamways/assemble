namespace Assemble.Scheme;

public class SchemeProcedure : SchemeCallable
{
    public SchemeProcedure(Environment closure, string[] parameters, SchemeObject[] body)
    {
        Closure = closure;
        Parameters = parameters;
        Body = body;
    }

    public Environment Closure { get; init; }

    public string[] Parameters { get; init; }

    public SchemeObject[] Body { get; init; }

    public override string Name => "procedure";

    public override SchemeObject Call(Environment e, SchemeObject arguments)
    {
        var closure = new Environment(Closure);

        var args = new List<SchemeObject>();

        if (arguments is SchemePair p)
            args.AddRange(p.AsEnumerable());
        else if (arguments is not SchemeEmptyList)
            args.Add(arguments);

        foreach (var (k, v) in Parameters.Zip(args))
            closure.Set(SchemeSymbol.FromString(k), v.Evaluate(e));

        return Body.Select(x => x.Evaluate(closure)).Last();
    }

    public override bool Equals(SchemeObject? other)
        => false;
}