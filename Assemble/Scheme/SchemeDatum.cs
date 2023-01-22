namespace Assemble.Scheme;

public abstract class SchemeDatum : IEquatable<SchemeDatum>
{
    public abstract bool Equals(SchemeDatum? other);

    public abstract string Print();

    public virtual T To<T>() where T : SchemeDatum
    {
        if (this is T t)
            return t;

        throw new Exception($"Type error: have {GetType()} but wanted {typeof(T)}");
    }
}