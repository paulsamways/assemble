using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class Interpreter
{
    private SchemeObject _accumulator = SchemeUndefined.Value;

    public Interpreter(SchemeDatum source, Environment? e = null)
    {
        Environment = e ?? Environment.Base();
        Ribs = new List<SchemeObject>();
        Frames = new Stack<Frame>();

        Instructions = new InstructionList();
        source.Compile(Instructions);
    }

    public InstructionList Instructions { get; }

    public Environment Environment { get; private set; }

    public List<SchemeObject> Ribs { get; private set; }

    public Stack<Frame> Frames { get; }

    public int Next { get; set; }

    public void Apply(Environment e, int next)
    {
        Environment = e;
        Ribs = new List<SchemeObject>();
        Next = next;
    }

    public void Frame(int next)
    {
        var frame = new Frame(
            Environment,
            Ribs,
            next
        );
        Frames.Push(frame);
        Ribs = new List<SchemeObject>();
    }

    public void Return()
    {
        var frame = Frames.Pop();

        Environment = frame.Environment;
        Ribs = frame.Ribs;
        Next = frame.Next;
    }

    public SchemeObject Run()
    {
        var xs = Instructions.ToArray();

        while (Next < xs.Length)
        {
            var n = Next;
            Next++;

            try
            {
                _accumulator = xs[n].Execute(_accumulator, this);
            }
            catch (Exception e)
            {
                throw new Exception($"Runtime error at instruction {n}", e);
            }
        }

        return _accumulator;
    }
}