namespace Scheme;

public abstract class SchemeDatum : SchemeObject
{
    public virtual SchemeSyntaxObject ToSyntaxObject()
        => new(this);

    public override SchemeDatum ToDatum()
        => this;
}