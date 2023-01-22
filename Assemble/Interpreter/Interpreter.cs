using Assemble.Interpreter.AST;

namespace Assemble.Interpreter;

public class Interpreter
{
    public Interpreter()
    {
        Context = new Context();
    }

    public Context Context { get; }

    public Expression Evaluate(string input)
    {
        var expression = ExpressionParser.Parse(input);
        return Evaluate(expression);
    }

    public Expression Evaluate(Expression expression)
    {
        return expression.Evaluate(Context);
    }
}