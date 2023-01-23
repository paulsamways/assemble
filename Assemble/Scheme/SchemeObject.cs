namespace Assemble.Scheme;

public abstract class SchemeObject
{
    public virtual T To<T>() where T : SchemeObject
    {
        if (this is T t)
            return t;

        throw new Exception($"Type error: have {GetType()} but wanted {typeof(T)}");
    }

    public abstract SchemeObject Evaluate(Environment e);

    public virtual string Write()
    {
        return $"<{GetType().Name}>";
    }
}