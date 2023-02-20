using System.Diagnostics.CodeAnalysis;
using Scheme.Interop;

namespace Scheme.Compiler;

public class Environment
{
    private readonly Environment? _parent;

    private readonly Dictionary<string, SchemeObject> _objects;

    public Environment(Environment? parent = null)
    {
        _objects = new();
        _parent = parent;
    }

    public IEnumerable<string> GetNames()
    {
        var names = new SortedSet<string>();

        foreach (var key in _objects.Keys)
            names.Add(key);

        if (_parent is not null)
        {
            foreach (var name in _parent.GetNames())
                names.Add(name);
        }

        return names;
    }

    public SchemeObject? Get(SchemeSymbol symbol)
    {
        if (_objects.TryGetValue(symbol.Value, out var o))
            return o;

        if (_parent is not null)
        {
            o = _parent.Get(symbol);

            if (o is not null)
                return o;
        }

        return null;
    }

    public bool TryGet(SchemeSymbol symbol, [NotNullWhen(true)] out SchemeObject? value)
    {
        value = Get(symbol);
        return value is not null;
    }

    public SchemeObject GetOrThrow(SchemeSymbol symbol)
    {
        var result = Get(symbol);
        if (result is null)
            throw new Exception("Unbound: " + symbol.Value);
        return result;
    }

    public void Set(SchemeSymbol symbol, SchemeObject o)
    {
        _objects[symbol.Value] = o;
    }

    public static Environment Base()
    {
        var e = new Environment();
        Reflect.LoadBuiltinProceduresFromAssembly<Environment>(e);
        return e;
    }
}