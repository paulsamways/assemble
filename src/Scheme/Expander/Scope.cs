namespace Scheme.Expander;

public sealed class Scope : IComparable<Scope>
{
    private static int _counter = 0;

    public Scope()
    {
        Id = _counter++;
        Bindings = new Dictionary<SchemeSymbol, Dictionary<HashSet<Scope>, Binding>>();
    }

    public int Id { get; }

    public Dictionary<SchemeSymbol, Dictionary<HashSet<Scope>, Binding>> Bindings { get; }

    public int CompareTo(Scope? other)
    {
        return Id.CompareTo(other?.Id);
    }
}