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

    public string Source { get; }

    public int Line { get; }

    public int Column { get; }

    public int Offset { get; }

    public int Length { get; }

    public ReadOnlySpan<char> Span =>
        Source.Substring(Offset, Length);
}