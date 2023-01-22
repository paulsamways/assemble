namespace Assemble.Interpreter.AST;

public sealed class DottedList : Expression
{
    public required Expression[] Head { get; init; }

    public required Expression Tail { get; init; }

    public override string Print()
    {
        throw new NotImplementedException();
    }
}