namespace Scheme.Expander;

public sealed class SourceInfo
{
    public SourceInfo(string source, int line, int column, int offset, int length)
    {
        Source = source;
        Line = line;
        Column = column;
        Offset = offset;
        Length = length;
    }

    public string Source { get; set; }

    public int Line { get; set; }

    public int Column { get; set; }

    public int Offset { get; set; }

    public int Length { get; set; }

    public ReadOnlySpan<char> Span =>
        Source.Substring(Offset, Length);
}