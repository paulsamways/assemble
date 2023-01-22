namespace Assemble.Interpreter.AST;

public sealed class Boolean : Expression
{
    public Boolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; init; }

    public override string Print()
    {
        return Value ? "#t" : "#f";
    }
}