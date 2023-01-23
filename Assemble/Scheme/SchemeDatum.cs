namespace Assemble.Scheme;

public abstract class SchemeDatum : SchemeObject, IEquatable<SchemeDatum>
{
    public abstract bool Equals(SchemeDatum? other);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        return Equals(obj as SchemeDatum);
    }
}