namespace Scheme;

public sealed class SchemeSyntaxObject : SchemeObject
{
    public SchemeSyntaxObject(SchemeDatum datum, SourceInfo source)
    {
        Datum = datum;
        Source = source;
    }
    public override string Name => "syntax";

    public SchemeDatum Datum { get; set; }

    public SourceInfo Source { get; set; }

    public override string ToString()
        => Datum.ToString();

    public override bool Equals(SchemeObject? other)
        => Datum.Equals(other);

    public override bool Same(SchemeObject other)
        => Datum.Same(other);

    public override int GetHashCode()
        => Datum.GetHashCode();
}