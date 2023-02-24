namespace Scheme.Expander;

using Environment = Interpreter.Environment;

public sealed class ExpandContext
{
    public ExpandContext(Namespace @namespace)
    {
        Namespace = @namespace;
        Environment = new Environment();
    }

    public List<Scope>? UseSiteScopes { get; set; }

    public Namespace Namespace { get; }

    public Interpreter.Environment Environment { get; }

    public bool OnlyImmediate { get; set; }

    public Scope? PostExpansionScope { get; set; }
}