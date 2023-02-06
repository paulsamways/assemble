using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class Interpreter
{
    public event EventHandler? Step;

    protected virtual void OnStep(EventArgs e)
    {
        Step?.Invoke(this, e);
    }

    public Interpreter(Environment? e = null)
    {
        Environment = e ?? Environment.Base();
        Accumulator = SchemeUndefined.Value;
        Ribs = new List<SchemeObject>();
        Continuations = new Dictionary<int, Frame>();
    }

    public SchemeObject Accumulator { get; set; }

    public Environment Environment { get; private set; }

    public List<SchemeObject> Ribs { get; private set; }

    public Frame? StackFrame { get; set; }

    public Dictionary<int, Frame> Continuations { get; set; }

    public Instruction? Next { get; set; }

    public void Apply(Environment e, Instruction next)
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

    public void Frame(Instruction next)
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

    public SchemeObject Run(SchemeDatum input)
    {
        var compiler = new Compiler();
        Next = compiler.Compile(input);

        while (Next is not null)
        {
            OnStep(EventArgs.Empty);

            try
            {
                Next.Execute(this);
            }
            catch (Exception e)
            {
                throw new Exception($"Runtime error at instruction {Next}", e);

            }
        }

        return Accumulator;
    }
}