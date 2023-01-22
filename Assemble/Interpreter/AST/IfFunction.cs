namespace Assemble.Interpreter.AST;

public sealed class IfFunction : Function
{
    public override Expression Apply(Context context, Expression[] arguments)
    {
        if (arguments.Length != 3)
            throw new Exception("wrong number of arguments to cond");

        return arguments[0].Evaluate(context).To<Boolean>().Value
            ? arguments[1].Evaluate(context)
            : arguments[2].Evaluate(context);
    }

    public override string Print()
    {
        return "<if>";
    }
}