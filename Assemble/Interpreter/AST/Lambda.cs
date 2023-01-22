namespace Assemble.Interpreter.AST;

public sealed class Lambda : Function
{
    public Lambda(string[] parameters, Expression[] body)
    {
        Parameters = parameters;
        Body = body;
    }

    public string[] Parameters { get; init; }

    public Expression[] Body { get; init; }

    public override Expression Apply(Context context, Expression[] arguments)
    {
        if (Parameters.Length != arguments.Length)
            throw new Exception("wrong number of arguments");

        var closure = new Context(context);
        closure.Set(Parameters.Zip(arguments));

        return Body
            .Select(x => x.Evaluate(closure))
            .Last();
    }

    public override string Print()
    {
        return "(lambda (" + string.Join(' ', Parameters) + ") ...)";
    }
}