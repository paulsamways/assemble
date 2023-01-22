namespace Assemble.Interpreter.AST;

public sealed class Number : Expression, IEquatable<Expression>
{
    public Number(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; init; }

    public bool Equals(Expression? other)
    {
        if (other is Number number)
            return Value == number.Value;

        return false;
    }

    public override string Print()
    {
        return Value.ToString();
    }
}