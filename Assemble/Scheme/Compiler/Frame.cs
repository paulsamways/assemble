using Assemble.Scheme.Compiler;

namespace Assemble.Scheme.Compiler;

public class Frame
{
    public Frame(Environment e, List<SchemeObject> ribs, int next, Frame? parent = null)
    {
        Environment = e;
        Ribs = ribs;
        Next = next;
        Parent = parent;
    }

    public Environment Environment { get; set; }

    public List<SchemeObject> Ribs { get; set; }

    public int Next { get; set; }

    public Frame? Parent { get; set; }
}