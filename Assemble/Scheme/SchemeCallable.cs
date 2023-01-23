namespace Assemble.Scheme;

public abstract class SchemeCallable : SchemeObject
{
    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }

    public abstract SchemeObject Call(Environment e, SchemeObject arguments);
}