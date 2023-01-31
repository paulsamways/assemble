using Assemble.Scheme.Compiler;

namespace Assemble.Scheme.Compiler;

public class Frame
{
    public Frame(Environment e, List<SchemeObject> ribs, int next)
    {
        Environment = e;
        Ribs = ribs;
        Next = next;
    }

    public Environment Environment { get; set; }

    public List<SchemeObject> Ribs { get; set; }

    public int Next { get; set; }
}