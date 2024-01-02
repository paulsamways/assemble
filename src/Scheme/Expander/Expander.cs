namespace Scheme.Expander;

using System.Diagnostics.CodeAnalysis;
using static Scheme.Match;

public class Expander
{
    public static readonly SchemeSymbol Variable = SchemeSymbol.Gensym("variable");

    private static readonly HashSet<SchemeSymbol> CoreForms = new(new SchemeSymbol[] {
        SchemeSymbol.Form.Lambda,
        SchemeSymbol.Form.LetSyntax,
        SchemeSymbol.Form.QuoteSyntax,
        SchemeSymbol.Form.Quote,
    });

    private static readonly Scope CoreScope = new();
    private static readonly HashSet<Scope> CoreScopeSet = new(new Scope[] { CoreScope });

    private readonly Interpreter.VM _vm;
    private readonly Namespace _namespace;

    private readonly Dictionary<SchemeSymbol, SchemeObject> _corePrimitives;
    private readonly Dictionary<SchemeSymbol, SchemeCoreForm> _coreForms;

    public Expander()
        : this(Interpreter.Environment.Base())
    {

    }

    public Expander(Interpreter.Environment e)
        : this(e, new Interpreter.VM(e))
    {

    }

    public Expander(Interpreter.Environment e, Interpreter.VM vm)
    {
        _vm = vm;

        _namespace = new Namespace();

        _corePrimitives = new Dictionary<SchemeSymbol, SchemeObject>();
        _coreForms = new Dictionary<SchemeSymbol, SchemeCoreForm>();

        foreach (var v in e.GetNames())
        {
            var sym = SchemeSymbol.FromString(v);
            _corePrimitives.Add(sym, e.GetOrThrow(sym));
        }
    }



    private void AddBinding(SchemeSyntaxObject id, Binding binding)
        => id.AddBindingInScopes(binding);

    private SchemeSymbol AddLocalBinding(SchemeSyntaxObject id)
    {
        var key = SchemeSymbol.Gensym(id.Datum.To<SchemeSymbol>().Value);
        AddBinding(id, new LocalBinding(key));
        return key;
    }

    private void AddCoreBinding(SchemeSymbol symbol)
        => AddBinding(new SchemeSyntaxObject(symbol, CoreScopeSet), new TopLevelBinding(symbol));

    private void AddCorePrimitive(SchemeSymbol symbol, SchemeObject value)
    {
        AddCoreBinding(symbol);
        _corePrimitives.Add(symbol, value);
    }
    private void AddCoreForm(SchemeSymbol symbol, SchemeCoreForm value)
    {
        AddCoreBinding(symbol);
        _coreForms.Add(symbol, value);
    }

    public void DeclareCoreTopLevel(Namespace ns)
    {
        foreach (var primitive in _corePrimitives)
            ns.SetVariable(primitive.Key, primitive.Value);

        foreach (var form in _coreForms)
            ns.SetTransformer(form.Key, form.Value);
    }

    private void NamespaceSyntaxIntroduce(SchemeObject o)
        => o.Visit(new SchemeObjectVisitor(false)
        {
            OnSchemeSyntaxObject = (stx, datum) =>
            {
                if (datum is SchemeSymbol)
                {
                    stx.AddScope(CoreScope);
                    return stx;
                }
                return datum;
            }
        });

    // main.rkt#35
    public SchemeObject Expand(SchemeObject o)
    {
        var stx = o.ToSyntax();
        NamespaceSyntaxIntroduce(stx);

        var expanded = Expand(stx, new ExpandContext(CoreScopeSet, _namespace));

        return Compile(expanded, _namespace);
    }

    // expand.rkt#26
    private SchemeObject Expand(SchemeObject o, ExpandContext ctx)
    {
        if (TryMatch(o, Identifier, out var stx))
            return ExpandIdentifier(stx, ctx);
        else
            return ExpandImplicit(SchemeSymbol.FromString("#%datum"), o.To<SchemeSyntaxObject>(), ctx);
        // else if (TryMatch(o, Application, out var app))
        // {
        //     return ExpandApplication(app.Datum.To<SchemePair>(), e);
        // }
        // else if (TryMatch(o, AnyPair, out var pair))
        // {

        // }
        // else
        // {
        //     return new SchemePair(
        //         new SchemeSyntaxObject(SchemeSymbol.Form.Quote, CoreScope),
        //         new SchemePair(o, SchemeEmptyList.Value));
        // }

        // throw new Exception("bad syntax: " + o.ToString());
        throw new NotImplementedException();
    }

    private SchemeObject ExpandIdentifier(SchemeSyntaxObject s, ExpandContext ctx)
    {
        var id = s.Datum.To<SchemeSymbol>();

        if (!s.TryResolveBinding(out Binding? binding))
            return ExpandImplicit(SchemeSymbol.FromString("#%top"), s, ctx);

        return Dispatch(ctx.Lookup(binding, id), s, ctx);


        // if (_bindings.TryResolve(o, out SchemeSymbol? b))
        // {
        //     if (_coreEnvironment.TryGet(b, out SchemeObject? _))
        //         return o;

        //     if (CoreForms.Contains(b))
        //         throw new Exception("bad syntax");

        //     if (e.TryGet(b, out SchemeObject? eo) && eo is SchemeSymbol v)
        //     {
        //         if (v.Equals(Variable))
        //             return o;

        //         throw new Exception("bad synax");
        //     }

        //     throw new Exception("out of context");
        // }

        // throw new Exception("free variable");
    }

    private SchemeSyntaxObject Dispatch(SchemeObject t, SchemeSyntaxObject s, ExpandContext ctx)
    {
        return t switch
        {
            SchemeCoreForm => throw new NotImplementedException(),
            SchemeTransformer => throw new NotImplementedException(),
            SchemeSymbol sym when sym.Equals(Binding.Variable) => s,
            _ => throw new Exception("Illegal use of syntax"),
        };
    }

    private SchemeObject ExpandImplicit(SchemeSymbol symbol, SchemeSyntaxObject o, ExpandContext ctx)
    {
        var id = new SchemeSyntaxObject(symbol, o.Scope);
        if (id.TryResolveBinding(out var binding) && ctx.TryLookup(binding, symbol, out var t))
        {
            switch (t)
            {
                case SchemeCoreForm form:
                    return form.Expand(o, ctx);
                case SchemeTransformer:

                    return null!;
            }
        }

        throw new Exception($"no transformer binding for {symbol}");
    }

    private SchemeObject Compile(SchemeObject o, Namespace ns)
    {
        if (TryMatch(o, Syntax(AnyPair), out var p))
        {
            throw new NotImplementedException();
        }
        else if (TryMatch(o, Identifier, out var identifier))
        {
            if (identifier.TryResolveBinding(out var b))
            {
                switch (b)
                {
                    case TopLevelBinding:
                        if (ns.TryGetVariable(b.Symbol, out var v))
                            return v;
                        break;
                    case LocalBinding:

                        break;

                }
            }

            throw new Exception("missing a binding after expansion: " + o.ToString());
        }
        else
        {
            throw new NotImplementedException();
        }

    }

    // private SchemeObject ExpandApplication(SchemePair p, Interpreter.Environment e)
    // {
    //     if (p.Car is SchemeSyntaxObject id && id.Datum is SchemeSymbol)
    //     {
    //         var binding = _bindings.Resolve(id);

    //         if (binding.Equals(SchemeSymbol.Form.Lambda))
    //             return ExpandLambda(p, e);

    //         if (binding.Equals(SchemeSymbol.Form.LetSyntax))
    //             return ExpandLetSyntax(p, e);

    //         if (binding.Equals(SchemeSymbol.Form.Quote) || binding.Equals(SchemeSymbol.Form.QuoteSyntax))
    //             return p;

    //         var eo = e.GetOrThrow(binding);

    //         if (eo is SchemeProcedure macro)
    //         {
    //             var ts = new Scope();
    //             AddScopeRecursive(p, ts);
    //             var result = _vm.Run(macro, p);
    //             FlipScopeRecursive(p, ts);

    //             return Expand(result, e);
    //         }
    //     }

    //     return SchemePair.FromEnumerable(p.ToEnumerable(true).Select(x => Expand(x, e)));
    // }

    // private SchemeObject ExpandLetSyntax(SchemePair p, Interpreter.Environment e)
    // {
    //     var els = p.ToEnumerable(true).ToArray();
    //     var letSyntaxSym = els[0];
    //     var bindings = els[1].To<SchemePair>().ToEnumerable(true).Select(x => x.To<SchemePair>());
    //     var body = els[2];

    //     var scope = new Scope();
    //     var env = new Interpreter.Environment(e);

    //     foreach (var pair in bindings)
    //     {
    //         var lhs = pair.Car.To<SchemeSyntaxObject>();
    //         AddScopeRecursive(lhs, CoreScope);
    //         var binding = _bindings.Add(lhs);

    //         var rhs = pair.Cdr.To<SchemePair>().Car;
    //         var rhsE = Expand(rhs, new Interpreter.Environment());
    //         var rhsC = Compile(rhsE);
    //         var rhsV = _vm.Run(rhsC.To<SchemeDatum>());

    //         env.Set(binding, rhsV);
    //     }

    //     AddScopeRecursive(body, scope);

    //     return Expand(body, env);
    // }

    // private SchemeObject ExpandLambda(SchemePair p, Interpreter.Environment e)
    // {
    //     var scope = new Scope();

    //     var els = p.ToEnumerable(true).ToArray();

    //     var lambdaSym = els[0];
    //     var args = els[1]
    //         .To<SchemePair>()
    //         .ToEnumerable(true);
    //     var body = els[2];

    //     var bodyEnv = new Interpreter.Environment(e);
    //     foreach (var arg in args)
    //     {
    //         AddScopeRecursive(arg, scope);

    //         bodyEnv.Set(_bindings.Add(arg.To<SchemeSyntaxObject>()), Variable);
    //     }

    //     AddScopeRecursive(body, scope);
    //     var expandedBody = Expand(body, bodyEnv);

    //     return SchemePair.FromEnumerable(new SchemeObject[] {
    //         lambdaSym,
    //         SchemePair.FromEnumerable(args),
    //         expandedBody
    //     });
    // }

    // private SchemeObject Compile(SchemeObject o)
    // {
    //     if (o is SchemePair p)
    //     {
    //         var els = p.ToEnumerable(true).ToArray();

    //         if (p.Car is SchemeSyntaxObject s && s.Datum is SchemeSymbol sym)
    //         {
    //             if (_bindings.TryResolve(s, out SchemeSymbol? coreSym))
    //             {
    //                 if (coreSym.Equals(SchemeSymbol.Form.Lambda))
    //                 {
    //                     return SchemePair.FromEnumerable(new SchemeObject[] {
    //                         SchemeSymbol.Form.Lambda,
    //                         SchemePair.FromEnumerable(
    //                             els[1]
    //                                 .To<SchemePair>()
    //                                 .ToEnumerable(true)
    //                                 .Select(x => _bindings.Resolve(x.To<SchemeSyntaxObject>()))),
    //                         Compile(els[2])
    //                     });
    //                 }
    //                 else if (coreSym.Equals(SchemeSymbol.Form.Quote))
    //                 {
    //                     return SchemePair.FromEnumerable(new SchemeObject[] {
    //                         SchemeSymbol.Form.Quote,
    //                         els[1].To<SchemeDatum>().DatumToSyntax(s)
    //                     });
    //                 }
    //                 else if (coreSym.Equals(SchemeSymbol.Form.QuoteSyntax))
    //                 {
    //                     return SchemePair.FromEnumerable(new SchemeObject[] {
    //                         SchemeSymbol.Form.Quote,
    //                         els[1]
    //                     });
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             return SchemePair.FromEnumerable(els.Select(Compile));
    //         }
    //     }
    //     else if (o is SchemeSyntaxObject s && s.Datum is SchemeSymbol)
    //     {
    //         if (_bindings.TryResolve(s, out SchemeSymbol? binding))
    //             return binding;
    //     }

    //     throw new Exception("Bad syntax");
    // }

    // private static void AddScopeRecursive(SchemeObject x, Scope s)
    // => x.Visit(new SchemeObjectVisitor(false)
    // {
    //     OnSchemeSyntaxObject = (stx, datum) =>
    //     {
    //         stx.AddScope(s);
    //         return stx;
    //     }
    // });

    // private static void FlipScopeRecursive(SchemeObject x, Scope s)
    //     => x.Visit(new SchemeObjectVisitor(false)
    //     {
    //         OnSchemeSyntaxObject = (stx, datum) =>
    //         {
    //             stx.FlipScope(s);
    //             return stx;
    //         }
    //     });
}