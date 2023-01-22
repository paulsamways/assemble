namespace Assemble.Interpreter.AST;

public sealed class SetPrimitiveFunction : Function
{
    public override Expression Apply(Context context, Expression[] arguments)
    {
        if (arguments.Length != 2)
            throw new Exception("wrong number of arguments to set!");

        var atom = arguments[0].To<Atom>();
        var value = arguments[1].Evaluate(context);

        context.Set(atom.Name, value);

        return value;
    }

    public override string Print()
    {
        return "<set!>";
    }
}