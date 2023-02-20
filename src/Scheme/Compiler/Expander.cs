namespace Scheme.Compiler;

public class Expander
{
    public static readonly SchemeSymbol Variable = SchemeSymbol.Gensym("variable");
    private readonly BindingTable _bindings;
    private readonly VM _vm;
    private readonly Environment _coreEnvironment;
    private readonly HashSet<SchemeSymbol> _coreForms = new(new SchemeSymbol[] {
        SchemeSymbol.Form.Lambda,
        SchemeSymbol.Form.LetSyntax,
        SchemeSymbol.Form.QuoteSyntax,
        SchemeSymbol.Form.Quote,
    });
    private readonly Scope _coreScope = new();

    public Expander()
    {
        _coreEnvironment = Environment.Base();
        _vm = new VM(_coreEnvironment);
        _bindings = new BindingTable();

        foreach (var core in _coreForms.Union(_coreEnvironment.GetNames().Select(SchemeSymbol.FromString)))
            _bindings.Add(new SchemeSyntaxObject(core, _coreScope), core);
    }

    private static void AddScopeRecursive(SchemeObject x, Scope s)
        => x.Visit(new SchemeObjectVisitor(false)
        {
            OnSchemeSyntaxObject = (stx, datum) =>
            {
                stx.AddScope(s);
                return stx;
            }
        });

    private static SchemeObject ToDatum(SchemeObject o)
        => o.Visit(new SchemeObjectVisitor()
        {
            OnSchemeSyntaxObject = (stx, datum) => datum
        });

    public SchemeObject Expand(SchemeObject o)
    {
        var x = o.Visit(new SchemeObjectVisitor()
        {
            OnSchemeSyntaxObject = (stx, datum) =>
            {
                if (datum is SchemeSymbol)
                {
                    stx.AddScope(_coreScope);
                    return stx;
                }
                return datum;
            }
        });

        return Expand(x, _coreEnvironment);
    }

    private SchemeObject Expand(SchemeObject o, Environment e)
    {
        if (o is SchemeSyntaxObject stx && stx.Datum is SchemeSymbol)
        {
            return ExpandIdentifier(stx, e);
        }
        else if (o is SchemePair p)
        {
            return ExpandApplication(p, e);
        }

        throw new Exception("bad syntax: " + o.ToString());
    }

    private SchemeSyntaxObject ExpandIdentifier(SchemeSyntaxObject o, Environment e)
    {
        if (_bindings.TryResolve(o, out SchemeSymbol? b))
        {
            if (_coreEnvironment.TryGet(b, out SchemeObject? _))
                return o;

            if (_coreForms.Contains(b))
                throw new Exception("bad syntax");

            if (e.TryGet(b, out SchemeObject? eo) && eo is SchemeSymbol v)
            {
                if (v.Equals(Variable))
                    return o;

                throw new Exception("bad synax");
            }

            throw new Exception("out of context");
        }

        throw new Exception("free variable");
    }

    private SchemeObject ExpandApplication(SchemePair p, Environment e)
    {
        if (p.Car is SchemeSyntaxObject id && id.Datum is SchemeSymbol)
        {
            var binding = _bindings.Resolve(id);

            if (binding.Equals(SchemeSymbol.Form.Lambda))
                return ExpandLambda(p, e);

            if (binding.Equals(SchemeSymbol.Form.LetSyntax))
                return ExpandLetSyntax(p, e);

            if (binding.Equals(SchemeSymbol.Form.Quote) || binding.Equals(SchemeSymbol.Form.QuoteSyntax))
                return p;

            var eo = e.GetOrThrow(binding);

            if (eo is SchemeProcedure macro)
            {
                var ts = new Scope();
                AddScopeRecursive(p, ts);
                var result = _vm.Run(macro, p);

                // TODO: flip scope!!

                return Expand(result, e);
            }
        }

        return SchemePair.FromEnumerable(p.ToEnumerable(true).Select(x => Expand(x, e)));
    }

    private SchemeObject ExpandLetSyntax(SchemePair p, Environment e)
    {
        var els = p.ToEnumerable(true).ToArray();
        var letSyntaxSym = els[0];
        var bindings = els[1].To<SchemePair>().ToEnumerable(true).Select(x => x.To<SchemePair>());
        var body = els[2];

        var scope = new Scope();
        var env = new Environment(e);

        foreach (var pair in bindings)
        {
            var lhs = pair.Car.To<SchemeSyntaxObject>();
            AddScopeRecursive(lhs, _coreScope);
            var binding = _bindings.Add(lhs);

            var rhs = pair.Cdr.To<SchemePair>().Car;
            var rhsE = Expand(rhs, new Environment());
            var rhsC = Compile(rhsE);
            var rhsV = _vm.Run(rhsC.To<SchemeDatum>());

            env.Set(binding, rhsV);
        }

        AddScopeRecursive(body, scope);

        return Expand(body, env);
    }

    private SchemeObject ExpandLambda(SchemePair p, Environment e)
    {
        var scope = new Scope();

        var els = p.ToEnumerable(true).ToArray();

        var lambdaSym = els[0];
        var args = els[1]
            .To<SchemePair>()
            .ToEnumerable(true);
        var body = els[2];

        var bodyEnv = new Environment(e);
        foreach (var arg in args)
        {
            AddScopeRecursive(arg, scope);

            bodyEnv.Set(_bindings.Add(arg.To<SchemeSyntaxObject>()), Variable);
        }

        AddScopeRecursive(body, scope);
        var expandedBody = Expand(body, bodyEnv);

        return SchemePair.FromEnumerable(new SchemeObject[] {
            lambdaSym,
            SchemePair.FromEnumerable(args),
            expandedBody
        });
    }

    private SchemeObject Compile(SchemeObject o)
    {
        if (o is SchemePair p)
        {
            var els = p.ToEnumerable(true).ToArray();

            if (p.Car is SchemeSyntaxObject s && s.Datum is SchemeSymbol sym)
            {
                if (_bindings.TryResolve(s, out SchemeSymbol? coreSym))
                {
                    if (coreSym.Equals(SchemeSymbol.Form.Lambda))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Form.Lambda,
                            SchemePair.FromEnumerable(
                                els[1]
                                    .To<SchemePair>()
                                    .ToEnumerable(true)
                                    .Select(x => _bindings.Resolve(x.To<SchemeSyntaxObject>()))),
                            Compile(els[2])
                        });
                    }
                    else if (coreSym.Equals(SchemeSymbol.Form.Quote))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Form.Quote,
                            ToDatum(els[1])
                        });
                    }
                    else if (coreSym.Equals(SchemeSymbol.Form.QuoteSyntax))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Form.Quote,
                            els[1]
                        });
                    }
                }
            }
            else
            {
                return SchemePair.FromEnumerable(els.Select(Compile));
            }
        }
        else if (o is SchemeSyntaxObject s && s.Datum is SchemeSymbol)
        {
            if (_bindings.TryResolve(s, out SchemeSymbol? binding))
                return binding;
        }

        throw new Exception("Bad syntax");
    }
}