using System.Diagnostics.CodeAnalysis;

namespace Scheme.Expander;

public class Environment
{
    private readonly Dictionary<SchemeSymbol, SchemeSymbol> _values;

    public static readonly SchemeSymbol Variable = SchemeSymbol.Gensym("variable");

    public Environment(Environment? e = null)
    {
        _values = e is null ? new() : new(e._values);
    }

    public void Add(SchemeSymbol key, SchemeSymbol value)
        => _values.Add(key, value);

    public bool TryLookup(SchemeSymbol key, [NotNullWhen(true)] out SchemeSymbol? value)
        => _values.TryGetValue(key, out value);
}