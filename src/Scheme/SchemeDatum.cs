namespace Scheme;

public abstract class SchemeDatum : SchemeObject
{
    public SchemeObject DatumToSyntax(SchemeSyntaxObject? stxC = null, SchemeSyntaxObject? stxS = null, SchemeSyntaxObject? stxP = null)
    {
        SchemeSyntaxObject Wrap(SchemeDatum e) => new(e, stxS?.Source, stxC?.Scope);

        return Visit(new()
        {
            OnSchemeSyntaxObject = (x, _) => x,
            Otherwise = (x) => x is SchemeDatum datum ? Wrap(datum) : x
        });
    }
}