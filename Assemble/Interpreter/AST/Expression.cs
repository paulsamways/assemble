namespace Assemble.Interpreter.AST;

public abstract class Expression
{
    public virtual Expression Evaluate(Context context)
    {
        return this;
    }

    public abstract string Print();

    public virtual T To<T>() where T : Expression
    {
        if (this is T t)
            return t;

        throw new Exception($"Type error: have {GetType()} but wanted {typeof(T)}");
    }
}