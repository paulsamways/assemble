using System.Diagnostics.CodeAnalysis;

namespace Scheme.Expander;

public sealed class Namespace
{
    private readonly Dictionary<SchemeSymbol, SchemeObject> _variables;
    private readonly Dictionary<SchemeSymbol, SchemeTransformer> _transformers;

    public Namespace()
    {
        _variables = new Dictionary<SchemeSymbol, SchemeObject>();
        _transformers = new Dictionary<SchemeSymbol, SchemeTransformer>();
    }

    public void SetVariable(SchemeSymbol symbol, SchemeObject @object)
        => _variables[symbol] = @object;

    public void SetTransformer(SchemeSymbol symbol, SchemeTransformer transformer)
        => _transformers[symbol] = transformer;

    public bool TryGetVariable(SchemeSymbol symbol, [NotNullWhen(true)] out SchemeObject? @object)
        => _variables.TryGetValue(symbol, out @object);

    public bool TryGetTransformer(SchemeSymbol symbol, [NotNullWhen(true)] out SchemeTransformer? transformer)
        => _transformers.TryGetValue(symbol, out transformer);
}