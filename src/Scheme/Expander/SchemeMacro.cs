namespace Scheme.Expander;

public sealed class SchemeMacro : SchemeTransformer
{
    public SchemeMacro(SchemeProcedure procedure)
    {
        Procedure = procedure;
    }

    public override string Name => "macro";

    public SchemeProcedure Procedure { get; set; }

    public override bool Equals(SchemeObject? other)
        => Procedure.Equals((other as SchemeMacro)?.Procedure);

    public override SchemeObject Expand(SchemeSyntaxObject o, ExpandContext context)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
        => Procedure.GetHashCode();

    public override SchemeObject Visit(SchemeObjectVisitor v)
        => this;
}