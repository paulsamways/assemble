namespace Assemble.Interpreter.AST;

public sealed class List : Expression
{
    public required Expression[] Elements { get; init; }

    public override Expression Evaluate(Context context)
    {
        if (Elements.Length == 2 && Elements[0] is Atom atom && atom.Name == "quote")
            return Elements[1];

        if (Elements.Length > 0 && Elements[0] is Atom function)
        {
            return context
                .Get(function.Name)
                .To<Function>()
                .Apply(context, Elements.Skip(1).ToArray());
        }

        return this;
    }

    public override string Print()
    {
        return "(" + string.Join(' ', Elements.Select(x => x.Print())) + ")";
    }
}