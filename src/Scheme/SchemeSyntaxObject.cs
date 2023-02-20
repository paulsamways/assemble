using Scheme.Compiler;

namespace Scheme;

public class SchemeSyntaxObject : SchemeObject
{
    public SchemeSyntaxObject(SchemeDatum datum, params Scope[] scopes)
        : this(datum, null, scopes)
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
}