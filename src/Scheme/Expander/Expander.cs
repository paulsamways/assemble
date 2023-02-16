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

    private SchemeObject ExpandIdentifierApplicationForm(SchemeSyntaxObject o, Environment e)
    {
        var p = o.Datum.To<SchemePair>();
        var id = p.Car.To<SchemeSyntaxObject>();

        if (_bindings.TryResolve(id, out SchemeSymbol? binding))
        {
            if (binding.Equals(SchemeSymbol.Known.Lambda))
                return ExpandLambda(o, e);

            if (binding.Equals(SchemeSymbol.Known.LetSyntax))
                return ExpandLetSyntax(o, e);

            if (binding.Equals(SchemeSymbol.Known.Quote) || binding.Equals(SchemeSymbol.Known.QuoteSyntax))
                return o;

            if (e.TryLookup(binding, out SchemeObject? eo))
            {
                if (eo is SchemeProcedure)
                {
                    var ts = new Scope();
                    var x = o.AddScope(ts);
                    throw new NotImplementedException();
                }
                else if (eo is SchemeSymbol v)
                {
                    if (v.Equals(Environment.Variable))
                        return ExpandApplication(o, e);
                }
            }
        }

        throw new NotImplementedException();
    }

    private SchemeObject ExpandLetSyntax(SchemeSyntaxObject o, Environment e)
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

        var els = o.Datum.To<SchemePair>().ToEnumerable(true).ToArray();
        var letSyntaxSym = els[0];
        var bindings = els[1].To<SchemeSyntaxObject>().Datum.To<SchemePair>().ToEnumerable(true).Select(x => x.To<SchemeSyntaxObject>().Datum.To<SchemePair>());
        var body = els[2].To<SchemeSyntaxObject>();

        var scope = new Scope();
        var env = new Environment(e);

        foreach (var pair in bindings)
        {
            var lhs = pair.Car.To<SchemeSyntaxObject>().AddScope(scope);
            var binding = _bindings.Add(lhs);

            var rhs = pair.Cdr.To<SchemePair>().Car.To<SchemeSyntaxObject>();
            var rhsE = Expand(rhs, new Environment());
            var rhsC = Compile(rhsE);
            var rhsV = _vm.Run(rhsC.To<SchemeDatum>());

            env.Add(binding, rhsV);
        }

        return Expand(body.AddScope(scope), env);
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
                                    .To<SchemeSyntaxObject>()
                                    .Datum
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
                            els[1].ToDatum()
                        });
                    }
                    else if (coreSym.Equals(SchemeSymbol.Known.QuoteSyntax))
                    {
                        return SchemePair.FromEnumerable(new SchemeObject[] {
                            SchemeSymbol.Known.Quote,
                            els[1]
                        });
                    }
                    else
                    {
                        return SchemePair.FromEnumerable(els.Select(Compile));
                    }
                }
            }
        }
        else if (o is SchemeSyntaxObject s && s.Datum is SchemeSymbol)
        {
            if (_bindings.TryResolve(s, out SchemeSymbol? binding))
                return binding;
        }

        throw new Exception("Bad syntax");
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