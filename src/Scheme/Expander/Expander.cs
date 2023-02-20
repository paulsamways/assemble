using Scheme.Compiler;

namespace Scheme.Expander;

public class Expander
{
    public static readonly SchemeSymbol Variable = SchemeSymbol.Gensym("variable");
    private readonly BindingTable _bindings;
    private readonly VM _vm;
    private readonly Compiler.Environment _coreEnvironment;
    private readonly HashSet<SchemeSymbol> _coreForms = new(new SchemeSymbol[] {
        SchemeSymbol.Known.Lambda,
        SchemeSymbol.Known.LetSyntax,
        SchemeSymbol.Known.QuoteSyntax,
        SchemeSymbol.Known.Quote,
    });
    private readonly Scope _coreScope = new();

    public Expander()
    {
        _coreEnvironment = Compiler.Environment.Base();
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

    private SchemeObject Expand(SchemeObject o, Compiler.Environment e)
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
        // Wrong
        return o;
    }

    private SchemeSyntaxObject ExpandIdentifier(SchemeSyntaxObject o, Compiler.Environment e)
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

    private SchemeObject ExpandApplication(SchemePair p, Compiler.Environment e)
    {
        if (p.Car is SchemeSyntaxObject id && id.Datum is SchemeSymbol)
        {
            var binding = _bindings.Resolve(id);

            if (binding.Equals(SchemeSymbol.Known.Lambda))
                return ExpandLambda(p, e);

            if (binding.Equals(SchemeSymbol.Known.LetSyntax))
                return ExpandLetSyntax(p, e);

            if (binding.Equals(SchemeSymbol.Known.Quote) || binding.Equals(SchemeSymbol.Known.QuoteSyntax))
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

    private SchemeObject ExpandLetSyntax(SchemePair p, Compiler.Environment e)
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
        var env = new Compiler.Environment(e);

        foreach (var pair in bindings)
        {
            var lhs = pair.Car.To<SchemeSyntaxObject>();
            AddScopeRecursive(lhs, _coreScope);
            var binding = _bindings.Add(lhs);

            var rhs = pair.Cdr.To<SchemePair>().Car;
            var rhsE = Expand(rhs, new Compiler.Environment());
            var rhsC = Compile(rhsE);
            var rhsV = _vm.Run(rhsC.To<SchemeDatum>());

            env.Set(binding, rhsV);
        }

        AddScopeRecursive(body, scope);

        return Expand(body, env);
    }

    private SchemeObject ExpandLambda(SchemePair p, Compiler.Environment e)
    {
        var scope = new Scope();

        var els = p.ToEnumerable(true).ToArray();

        var lambdaSym = els[0];
        var args = els[1]
            .To<SchemePair>()
            .ToEnumerable(true);
        var body = els[2];

        var bodyEnv = new Compiler.Environment(e);
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
}