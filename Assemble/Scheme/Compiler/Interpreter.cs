using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class Interpreter
{
    private SchemeObject _accumulator = SchemeUndefined.Value;

    public Interpreter(Environment? e = null)
    {
        Environment = e ?? Environment.Base();
        Ribs = new List<SchemeObject>();
        Instructions = new InstructionList();
        Continuations = new Dictionary<int, Frame>();
    }

    public InstructionList Instructions { get; }

    public Environment Environment { get; private set; }

    public List<SchemeObject> Ribs { get; private set; }

    public Frame? StackFrame { get; set; }

    public Dictionary<int, Frame> Continuations { get; set; }

    public int Next { get; set; }

    public void Apply(Environment e, int next)
    {
        Environment = e;
        Ribs = new List<SchemeObject>();
        Next = next;
    }

    public void Conti(int key)
    {
        if (StackFrame is null)
            throw new Exception("no frame to capture");

        Continuations[key] = StackFrame;
    }

    public void Nuate(Frame frame)
    {
        StackFrame = frame;
    }

    public void Frame(int next)
    {
        StackFrame = new Frame(
            Environment,
            Ribs,
            next,
            StackFrame
        );

        Ribs = new List<SchemeObject>();
    }

    public void Return()
    {
        if (StackFrame is null)
            throw new Exception("no frame to return to");

        Environment = StackFrame.Environment;
        Ribs = StackFrame.Ribs;
        Next = StackFrame.Next;

        StackFrame = StackFrame.Parent;
    }

    public void Load(SchemeDatum input)
    {
        var compiler = new Compiler(Instructions);
        compiler.Compile(input);
    }

    public SchemeObject Run(SchemeDatum input)
    {
        Load(input);
        return Run();
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