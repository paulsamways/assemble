namespace Assemble.Interpreter.AST;

public sealed class PrimitiveFunction : Function
{
    public PrimitiveFunction(Func<Expression[], Expression> func)
    {
        Func = func;
    }

    public Func<Expression[], Expression> Func { get; init; }

    public override Expression Apply(Context context, Expression[] arguments)
    {
        return Func(arguments.Select(x => x.Evaluate(context)).ToArray());
    }

    public override string Print()
    {
        return "<primitive>";
    }
}