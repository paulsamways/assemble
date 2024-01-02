namespace Scheme.Expander;

using System.Diagnostics.CodeAnalysis;
using Environment = Interpreter.Environment;

public sealed class ExpandContext
{
    public ExpandContext(HashSet<Scope> coreScope, Namespace @namespace)
    {
        CoreScope = coreScope;
        Namespace = @namespace;
        Environment = new Environment();
    }

    public HashSet<Scope> CoreScope { get; }

    public List<Scope>? UseSiteScopes { get; set; }

    public Namespace Namespace { get; }

    public Environment Environment { get; }

    public bool OnlyImmediate { get; set; }

    public Scope? PostExpansionScope { get; set; }

    public bool TryLookup(Binding b, SchemeSymbol id, [NotNullWhen(true)] out SchemeObject? value)
    {
        switch (b)
        {
            case TopLevelBinding:
                value = Namespace.TryGetTransformer(id, out var t) ? t : Binding.Variable;
                return true;
            case LocalBinding:
                return Environment.TryGet(id, out value);
            default:
                value = null;
                return false;
        }
    }

    public SchemeObject Lookup(Binding b, SchemeSymbol id)
    {
        switch (b)
        {
            case TopLevelBinding:
                if (Namespace.TryGetTransformer(b.Symbol, out var t))
                    return t;

                return Binding.Variable;
            case LocalBinding:

                if (Environment.TryGet(b.Symbol, out var v))
                    return v;

                throw new Exception($"identifier used out of context: {id}");
            default:
                throw new Exception($"unknown binding type: {b.GetType()}");
        }
    }
}