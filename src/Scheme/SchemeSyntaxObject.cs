using System.Diagnostics.CodeAnalysis;
using Scheme.Expander;

namespace Scheme;

public class SchemeSyntaxObject : SchemeObject
{
    public SchemeSyntaxObject(SchemeDatum datum, HashSet<Scope> scopes)
        : this(datum, null, scopes.ToArray())
    {

    }

    public SchemeSyntaxObject(SchemeDatum datum, params Scope[] scopes)
        : this(datum, null, scopes)
    {

    }

    public SchemeSyntaxObject(SchemeDatum datum, SourceInfo? source, HashSet<Scope>? scopes)
        : this(datum, source, scopes is not null ? scopes.ToArray() : Array.Empty<Scope>())
    {

    }

    public SchemeSyntaxObject(SchemeDatum datum, SourceInfo? source, params Scope[] scopes)
    {
        Datum = datum;
        Source = source;

        Scope = new HashSet<Scope>(scopes);
    }

    public override string Name => "syntax";

    public SchemeDatum Datum { get; set; }

    public SourceInfo? Source { get; set; }

    public HashSet<Scope> Scope { get; }

    public void AddScope(Scope s)
        => Scope.Add(s);

    public void FlipScope(Scope s)
    {
        if (Scope.Contains(s))
            Scope.Remove(s);
        else
            Scope.Add(s);
    }

    public override string ToString()
        => Datum.ToString();

    public override bool Equals(SchemeObject? other)
        => Datum.Equals(other);

    public override bool Same(SchemeObject other)
        => Datum.Same(other);

    public override int GetHashCode()
        => Datum.GetHashCode();

    public override SchemeObject Visit(SchemeObjectVisitor v)
        => v.OnSchemeSyntaxObject(this, Datum.Visit(v).To<SchemeDatum>());

    public SchemeObject SyntaxToDatum()
        => Visit(new SchemeObjectVisitor()
        {
            OnSchemeSyntaxObject = (stx, datum) => datum
        });

    public static SchemeSyntaxObject Empty => new(SchemeEmptyList.Value);

    public void AddBindingInScopes(Binding binding)
    {
        var sym = Datum.To<SchemeSymbol>();

        if (Scope.Count == 0)
            throw new Exception("can't bind in empty scope set");

        var maxScope = Scope.MaxBy(x => x.Id)!;

        if (!maxScope.Bindings.TryGetValue(sym, out var b))
        {
            b = new();
            maxScope.Bindings.Add(sym, b);
        }

        b.Add(Scope, binding);
    }

    public bool TryResolveBinding([NotNullWhen(true)] out Binding? binding)
        => TryResolveBinding(false, out binding);

    public bool TryResolveBinding(bool exactly, [NotNullWhen(true)] out Binding? binding)
    {
        var candidates = FindAllMatchingBindings();
        var maxCandidate = candidates.MaxBy(x => x.Item1.Count);

        if (maxCandidate is not null)
        {
            foreach (var candidate in candidates)
                if (!candidate.Item1.IsSubsetOf(maxCandidate.Item1))
                    throw new Exception("Amgbiguous scope");

            if (!exactly || Scope.Count == maxCandidate.Item1.Count)
            {
                binding = maxCandidate.Item2;
                return true;
            }
        }

        binding = null;
        return false;
    }


    private IEnumerable<Tuple<HashSet<Scope>, Binding>> FindAllMatchingBindings()
    {
        var symbol = Datum.To<SchemeSymbol>();
        foreach (var scope in Scope)
            if (scope.Bindings.TryGetValue(symbol, out var bindings))
                foreach (var binding in bindings)
                    if (binding.Key.IsSubsetOf(Scope))
                        yield return new(binding.Key, binding.Value);
    }
}