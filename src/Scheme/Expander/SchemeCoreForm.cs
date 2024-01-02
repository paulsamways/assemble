namespace Scheme.Expander;

public abstract class SchemeCoreForm : SchemeTransformer
{
    public SchemeCoreForm()
    {
    }

    public override string Name => "core-form";

    public override bool Equals(SchemeObject? other)
        => false;

    public override int GetHashCode()
        => 71;

    public override SchemeObject Visit(SchemeObjectVisitor v)
        => this;
}