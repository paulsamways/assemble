using Scheme.Compiler;

namespace Scheme.Expander;

public class Expander
{
    private readonly BindingTable _bindings;
    private readonly VM _vm;

    private readonly HashSet<SchemeSymbol> _corePrimitives = new(new SchemeSymbol[] {
        SchemeSymbol.Known.Cons
    });

    private readonly HashSet<SchemeSymbol> _coreForms = new(new SchemeSymbol[] {
        SchemeSymbol.Known.Lambda,
        SchemeSymbol.Known.LetSyntax,
        SchemeSymbol.Known.QuoteSyntax,
        SchemeSymbol.Known.Quote,
    });

    private readonly Scope _coreScope = new();

    public Expander()
    {
        _bindings = new BindingTable();
        _vm = new VM();

        foreach (var core in _corePrimitives.Union(_coreForms))
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

        return Expand(x, new Environment());
    }

    public SchemeObject Expand(SchemeObject o, Environment e)
    {
        // Identifier
        if (o is SchemeSyntaxObject stx && stx.Datum is SchemeSymbol)
        {
            return ExpandIdentifier(stx, e);
        }
        else if (o is SchemePair p && p.Car is SchemeSyntaxObject sso && sso.Datum is SchemeSymbol)
        {
            return ExpandIdentifierApplicationForm(p, e);
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

            if (e.TryLookup(b, out SchemeObject? eo) && eo is SchemeSymbol v)
            {
                if (v.Equals(Environment.Variable))
                    return o;

                throw new Exception("bad synax");
            }

            throw new Exception("out of context");
        }

        throw new Exception("free variable");
    }

    private SchemeObject ExpandIdentifierApplicationForm(SchemePair p, Environment e)
    {
        var id = p.Car.To<SchemeSyntaxObject>();

        if (_bindings.TryResolve(id, out SchemeSymbol? binding))
        {
            if (binding.Equals(SchemeSymbol.Known.Lambda))
                return ExpandLambda(p, e);

            if (binding.Equals(SchemeSymbol.Known.LetSyntax))
                return ExpandLetSyntax(p, e);

            if (binding.Equals(SchemeSymbol.Known.Quote) || binding.Equals(SchemeSymbol.Known.QuoteSyntax))
                return p;

            if (e.TryLookup(binding, out SchemeObject? eo))
            {
                if (eo is SchemeProcedure macro)
                {
                    var ts = new Scope();
                    AddScopeRecursive(p, ts);
                    var result = _vm.Run(macro, p);

                    return Expand(result, e);
                }
                else if (eo is SchemeSymbol v)
                {
                    if (v.Equals(Environment.Variable))
                        return ExpandApplication(p, e);
                }
            }
        }

        throw new NotImplementedException();
    }

    private SchemeObject ExpandLetSyntax(SchemePair p, Environment e)
    {
        /*
            (define (expand-let-syntax s env)
                (match-define `(,let-syntax-id ([,lhs-id ,rhs])
                                    ,body)
                                s)
                (define sc (scope))
                ;; Add the new scope to each binding identifier:
                (define id (add-scope lhs-id sc))
                ;; Bind the left-hand identifier and generate a corresponding key
                ;; for the expand-time environment:
                (define binding (add-local-binding! id))
                ;; Evaluate compile-time expressions:
                (define rhs-val (eval-for-syntax-binding rhs))
                ;; Fill expansion-time environment:
                (define body-env (env-extend env binding rhs-val))
                ;; Expand body
                (expand (add-scope body sc) body-env))
        */

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

            env.Add(binding, rhsV);
        }

        AddScopeRecursive(body, scope);

        return Expand(body, env);
    }

    private SchemeObject Compile(SchemeObject o)
    {
        /*

            ;; Convert an expanded syntax object to an expression that is
            ;; represented by a plain S-expression.
            (define (compile s)
            (cond
            [(pair? s)
                (define core-sym (and (identifier? (car s))
                                    (resolve (car s))))
                (case core-sym
                [(lambda)
                (match-define `(,lambda-id (,id) ,body) s)
                `(lambda (,(resolve id)) ,(compile body))]
                [(quote)
                (match-define `(,quote-id ,datum) s)
                ;; Strip away scopes:
                `(quote ,(syntax->datum datum))]
                [(quote-syntax)
                (match-define `(,quote-syntax-id ,datum) s)
                ;; Preserve the complete syntax object:
                `(quote ,datum)]
                [else
                ;; Application:
                (map compile s)])]
            [(identifier? s)
                (resolve s)]
            [else
                (error "bad syntax after expansion:" s)]))

        */

        // if (o is SchemeSyntaxObject stx)
        //     o = stx.Datum;

        if (o is SchemePair p)
        {
            var els = p.ToEnumerable(true).ToArray();

            if (p.Car is SchemeSyntaxObject s && s.Datum is SchemeSymbol sym)
            {
                if (_bindings.TryResolve(s, out SchemeSymbol? coreSym))
                {
                    if (coreSym.Equals(SchemeSymbol.Known.Lambda))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Known.Lambda,
                            SchemePair.FromEnumerable(
                                els[1]
                                    .To<SchemePair>()
                                    .ToEnumerable(true)
                                    .Select(x => _bindings.Resolve(x.To<SchemeSyntaxObject>()))),
                            Compile(els[2])
                        });
                    }
                    else if (coreSym.Equals(SchemeSymbol.Known.Quote))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Known.Quote,
                            ToDatum(els[1])
                        });
                    }
                    else if (coreSym.Equals(SchemeSymbol.Known.QuoteSyntax))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Known.Quote,
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

            bodyEnv.Add(_bindings.Add(arg.To<SchemeSyntaxObject>()), Environment.Variable);
        }

        AddScopeRecursive(body, scope);
        var expandedBody = Expand(body, bodyEnv);

        return SchemePair.FromEnumerable(new SchemeObject[] {
            lambdaSym,
            SchemePair.FromEnumerable(args),
            expandedBody
        });
    }

    private SchemeObject ExpandApplication(SchemePair p, Environment e)
    {
        var xs = p
            .ToEnumerable(true)
            .Select(x => Expand(x, e));

        return SchemePair.FromEnumerable(xs);
    }
}