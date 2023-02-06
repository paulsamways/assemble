namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionClosure : Instruction
{
    public InstructionClosure(string[] parameters, Instruction body, Instruction next)
    {
        Parameters = parameters;
        Body = body;
        Next = next;
    }

    public string[] Parameters { get; set; }

    public Instruction Next { get; set; }

    public Instruction Body { get; set; }

    public SchemeDatum? Source { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Accumulator = new SchemeProcedure(interpreter.Environment, Parameters, Body);
        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"CLSR {Source?.Write()}";
    }
}