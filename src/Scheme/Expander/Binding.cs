using Environment = Scheme.Interpreter.Environment;

namespace Scheme.Expander;

public abstract class Binding
{
    public static readonly SchemeSymbol Variable = SchemeSymbol.Gensym("variable");

    public static bool IsVariable(SchemeSymbol s) => s.Equals(Variable);

    public static bool IsTransformer(SchemeObject o) => o is SchemeProcedure;

    public Binding(SchemeSymbol symbol)
    {
        Symbol = symbol;
    }

    public SchemeSymbol Symbol { get; }

    public abstract SchemeObject LookupIn(Environment e, Namespace ns, SchemeSymbol @id);
}

public sealed class TopLevelBinding : Binding
{
    public TopLevelBinding(SchemeSymbol symbol)
        : base(symbol)
    {

    }

    public override SchemeObject LookupIn(Environment e, Namespace ns, SchemeSymbol id)
    {
        if (ns.TryGetTransformer(Symbol, out var o))
            return o;

        return Variable;
    }
}

public sealed class LocalBinding : Binding
{
    public LocalBinding(SchemeSymbol symbol)
        : base(symbol)
    {

    }

    public override SchemeObject LookupIn(Environment e, Namespace ns, SchemeSymbol id)
    {
        if (e.TryGet(Symbol, out var o))
            return o;

        throw new Exception("identifier used out of context: " + id.Value);
    }
}