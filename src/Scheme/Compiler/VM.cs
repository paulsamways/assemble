using Scheme.Compiler.Instructions;

namespace Scheme.Compiler;

public class VM
{
    public VM(Environment? e = null)
    {
        Environment = e ?? Environment.Base();
        Accumulator = SchemeUndefined.Value;
        Ribs = new List<SchemeObject>();
    }

    public SchemeObject Accumulator { get; set; }

    public Environment Environment { get; private set; }

    public List<SchemeObject> Ribs { get; private set; }

    public Frame? StackFrame { get; set; }

    public Instruction? Next { get; set; }

    public event EventHandler? Step;

    protected virtual void OnStep(EventArgs e)
    {
        Step?.Invoke(this, e);
    }

    public void Apply(Environment e, Instruction next)
    {
        Environment = e;
        Ribs = new List<SchemeObject>();
        Next = next;
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

    private SchemeObject Run()
    {
        while (Next is not null)
        {
            OnStep(EventArgs.Empty);

            Next.Execute(this);
        }

        return Accumulator;
    }

    public SchemeObject Run(SchemeDatum input)
    {
        var compiler = new Compiler();
        Next = compiler.Compile(input);

        return Run();
    }

    public SchemeObject Run(SchemeProcedure procedure, params SchemeObject[] arguments)
    {
        Next = new Constant(procedure, new Apply());

        foreach (var arg in arguments)
            Next = new Constant(arg.To<SchemeDatum>(), new Argument(Next));

        Next = new Instructions.Frame(new Halt(), Next);

        return Run();
    }
}