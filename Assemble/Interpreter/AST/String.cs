namespace Assemble.Interpreter.AST;

public sealed class String : Expression
{
    public required string Value { get; init; }

    public override string Print()
    {
        return "\"" + Value + "\"";
    }
}