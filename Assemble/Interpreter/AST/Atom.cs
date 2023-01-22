namespace Assemble.Interpreter.AST;

public sealed class Atom : Expression
{
    public Atom(string name)
    {
        Name = name;
    }

    public string Name { get; init; }

    public override string Print()
    {
        return Name;
    }

    public override Expression Evaluate(Context context)
    {
        return context.Get(Name);
    }

    public static Atom Quote => new("quote");
}