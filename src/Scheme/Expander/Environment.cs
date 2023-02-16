using System.Diagnostics.CodeAnalysis;

namespace Scheme.Expander;

public class Environment
{
    private readonly Dictionary<SchemeSymbol, SchemeObject> _values;

    public static readonly SchemeSymbol Variable = SchemeSymbol.Gensym("variable");

    public Environment(Environment? e = null)
    {
        _values = e is null ? new() : new(e._values);
    }

    public void Add(SchemeSymbol key, SchemeObject value)
        => _values.Add(key, value);

    public bool TryLookup(SchemeSymbol key, [NotNullWhen(true)] out SchemeObject? value)
        => _values.TryGetValue(key, out value);
}