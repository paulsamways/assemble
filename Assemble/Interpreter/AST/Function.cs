namespace Assemble.Interpreter.AST;

public abstract class Function : Expression
{
    public abstract Expression Apply(Context context, Expression[] arguments);
}