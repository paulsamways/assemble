using Scheme.Expander;

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


    private SchemeSyntaxObject AdjustScope(Scope s, Func<HashSet<Scope>, Scope, HashSet<Scope>> op)
    {
        var newScope = op(Scope, s);

        if (Datum is SchemePair p)
        {
            var car = p.Car;
            var cdr = p.Cdr;

            if (car is SchemeSyntaxObject s1)
                car = s1.AdjustScope(s, op);

            if (cdr is SchemeSyntaxObject s2)
                cdr = s2.AdjustScope(s, op);

            return new SchemeSyntaxObject(new SchemePair(car, cdr), newScope.ToArray());
        }
        else
        {
            return new SchemeSyntaxObject(Datum, newScope.ToArray());
        }
    }

    public SchemeSyntaxObject AddScope(Scope s)
        => AdjustScope(s, (scopes, scope) => new HashSet<Scope>(scopes) { scope });

    public SchemeSyntaxObject FlipScope(Scope s)
        => AdjustScope(s, (scopes, scope) =>
        {
            var s = new HashSet<Scope>(scopes);

            if (s.Contains(scope))
                s.Remove(scope);
            else
                s.Add(scope);

            return s;
        });

    public override string ToString()
        => Datum.ToString();

    public override SchemeDatum ToDatum()
        => Datum.ToDatum();

    public override bool Equals(SchemeObject? other)
        => Datum.Equals(other);

    public override bool Same(SchemeObject other)
        => Datum.Same(other);

    public override int GetHashCode()
        => Datum.GetHashCode();
}