namespace Scheme.Expander;

public abstract class SchemeTransformer : SchemeObject
{
    public abstract SchemeObject Expand(SchemeSyntaxObject o, ExpandContext context);
}