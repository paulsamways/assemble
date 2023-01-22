namespace Assemble.Interpreter.AST;

public sealed class LambdaFunction : Function
{
    public override Expression Apply(Context context, Expression[] arguments)
    {
        if (arguments.Length < 2)
            throw new Exception("wrong number of arguments for lambda");

        var ps = arguments[0]
            .To<List>()
            .Elements
            .Select(x => x.To<Atom>().Name)
            .ToArray();

        var body = arguments.Skip(1).ToArray();

        return new Lambda(ps, body);
    }

    public override string Print()
    {
        return "<lambda>";
    }
}