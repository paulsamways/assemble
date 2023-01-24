namespace Assemble.Scheme;

public abstract class SchemeBuiltin : SchemeCallable
{
    public override bool Equals(SchemeObject? other)
        => ReferenceEquals(this, other);
}