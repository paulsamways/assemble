namespace Scheme.Expander;

public class Expander
{
    private readonly BindingTable _bindings;

    private readonly HashSet<SchemeSymbol> _corePrimitives = new(
        new string[] { "cons" }.Select(SchemeSymbol.FromString));

    private readonly HashSet<SchemeSymbol> _coreForms = new(
        new string[] { "lambda", "quote" }.Select(SchemeSymbol.FromString));

    private readonly Scope _coreScope = new();

    public Expander()
    {
        _bindings = new BindingTable();

        foreach (var core in _corePrimitives.Union(_coreForms))
            _bindings.Add(new SchemeSyntaxObject(core, _coreScope), core);
    }

    public SchemeObject Expand(SchemeSyntaxObject o)
    {
        return Expand(o.AddScope(_coreScope), new Environment());
    }

    public SchemeObject Expand(SchemeSyntaxObject o, Environment e)
    {
        // Identifier
        if (o.Datum is SchemeSymbol)
        {
            return ExpandIdentifier(o, e);
        }
        else if (o.Datum is SchemePair p && p.Car.To<SchemeSyntaxObject>().Datum is SchemeSymbol)
        {
            return ExpandIdentifierApplicationForm(o, e);
        }
        else
        {
            return o;
        }

        throw new Exception("Bad syntax");
    }

    private SchemeSyntaxObject ExpandIdentifier(SchemeSyntaxObject o, Environment e)
    {
        if (_bindings.TryResolve(o, out SchemeSymbol? b))
        {
            if (_corePrimitives.Contains(b))
                return o;

            if (_coreForms.Contains(b))
                throw new Exception("bad syntax");

            if (e.TryLookup(b, out SchemeSymbol? v))
            {
                if (v.Equals(Environment.Variable))
                    return o;

                throw new Exception("bad synax");
            }

            throw new Exception("out of context");
        }

        throw new Exception("free variable");
    }

    private SchemeObject ExpandIdentifierApplicationForm(SchemeSyntaxObject o, Environment e)
    {
        var p = o.Datum.To<SchemePair>();
        var id = p.Car.To<SchemeSyntaxObject>();

        if (_bindings.TryResolve(id, out SchemeSymbol? binding))
        {
            if (binding.Equals(SchemeSymbol.Known.Lambda))
                return ExpandLambda(o, e);

            if (binding.Equals(SchemeSymbol.Known.Quote))
                return o;

            if (e.TryLookup(binding, out SchemeSymbol? v))
            {
                if (v.Equals(Environment.Variable))
                    return ExpandApplication(o, e);
            }
        }

        throw new NotImplementedException();
    }

    private SchemeObject ExpandLambda(SchemeSyntaxObject o, Environment e)
    {
        var scope = new Scope();

        var els = o.Datum.To<SchemePair>().ToEnumerable(true).ToArray();

        var lambdaSym = els[0];
        var args = els[1]
            .To<SchemeSyntaxObject>()
            .Datum
            .To<SchemePair>()
            .ToEnumerable(true)
            .Select(x => x.To<SchemeSyntaxObject>().AddScope(scope))
            .ToArray();
        var body = els[2].To<SchemeSyntaxObject>();


        var bodyEnv = new Environment(e);
        foreach (var arg in args)
            bodyEnv.Add(_bindings.Add(arg), Environment.Variable);

        var expandedBody = Expand(body.AddScope(scope), bodyEnv);

        return SchemePair.FromEnumerable(new SchemeObject[] {
            lambdaSym,
            SchemePair.FromEnumerable(args),
            expandedBody
        });
    }

    private SchemeObject ExpandApplication(SchemeSyntaxObject o, Environment e)
    {
        var xs = o
            .Datum
            .To<SchemePair>()
            .ToEnumerable(true)
            .Select(x => Expand(x.To<SchemeSyntaxObject>(), e));

        return SchemePair.FromEnumerable(xs);
    }
}