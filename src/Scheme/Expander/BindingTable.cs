using System.Diagnostics.CodeAnalysis;

namespace Scheme.Expander;

public class BindingTable
{
    private readonly Dictionary<SchemeSyntaxObject, SchemeSymbol> _bindings;

    public BindingTable()
    {
        _bindings = new Dictionary<SchemeSyntaxObject, SchemeSymbol>();
    }

    public void Add(SchemeSyntaxObject o, SchemeSymbol s)
        => _bindings.Add(o, s);

    public SchemeSymbol Add(SchemeSyntaxObject o)
    {
        if (o.Datum is SchemeSymbol s)
        {
            var gs = SchemeSymbol.Gensym(s.Value);
            Add(o, gs);
            return gs;
        }

        throw new Exception("SchemeSyntaxObject did not contain a SchemeSymbol");
    }

    public SchemeSymbol Resolve(SchemeSyntaxObject o)
    {
        if (TryResolve(o, out SchemeSymbol? s))
            return s;

        throw new Exception("could not resolve binding: " + o.ToString());
    }

    public bool TryResolve(SchemeSyntaxObject o, [NotNullWhen(true)] out SchemeSymbol? binding)
    {
        // (cond
        //    [(pair? candidate-ids)
        //     (define max-id
        //       (argmax (compose set-count syntax-scopes)
        //               candidate-ids))
        //     (check-unambiguous max-id candidate-ids)
        //     (hash-ref all-bindings max-id)]
        //    [else #f]))

        binding = null;

        var candidates = FindAllMatching(o);
        var candidateWithMaxScope = candidates.MaxBy(x => x.Scope.Count);

        if (candidateWithMaxScope is null)
            return false;

        if (candidates.Any(x => !x.Scope.IsSubsetOf(candidateWithMaxScope.Scope)))
            throw new Exception("ambigiuous");

        binding = _bindings[candidateWithMaxScope];
        return true;
    }

    private IEnumerable<SchemeSyntaxObject> FindAllMatching(SchemeSyntaxObject o)
    {
        // #:when (and (eq? (syntax-e c-id) (syntax-e id)) // datums are eq
        //            (subset? (syntax-scopes c-id) (syntax-scopes id)) /

        return _bindings
            .Keys
            .Where(x => x.Datum.Equals(o.Datum) && x.Scope.IsSubsetOf(o.Scope));
    }
}