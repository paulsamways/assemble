namespace Scheme.Expander;

using static Scheme.Match;
using static SchemeSymbol.Form;

public sealed class SchemeCoreFormDatum : SchemeCoreForm
{
    public override string Name => "#%datum";

    public override bool Equals(SchemeObject? other)
        => false;

    public override SchemeObject Expand(SchemeSyntaxObject o, ExpandContext context)
    {
        var (_, (_, _, datum)) = MatchOrThrow(o, Syntax(Pair(Symbol(Name), AnyDatum)));

        return SchemePair.List(
            Quote.ToSyntax(context.CoreScope),
            datum
        ).ToSyntax(o.Scope, o.Source);
    }

    public override int GetHashCode()
        => 71;

    public override SchemeObject Visit(SchemeObjectVisitor v)
        => this;


}