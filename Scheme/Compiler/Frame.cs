namespace Scheme.Compiler;

public class Frame
{
    public Frame(Environment e, List<SchemeObject> ribs, Instruction next, Frame? parent = null)
    {
        Environment = e;
        Ribs = ribs;
        Next = next;
        Parent = parent;
    }

    public Environment Environment { get; set; }

    public List<SchemeObject> Ribs { get; set; }

    public Instruction Next { get; set; }

    public Frame? Parent { get; set; }

    public int Depth =>
        1 + ((Parent?.Depth) ?? 0);
}