namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionConstant : Instruction
{
    public InstructionConstant(SchemeDatum datum, Instruction next)
    {
        Datum = datum;
        Next = next;
    }

    public SchemeDatum Datum { get; }

    public Instruction Next { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Accumulator = Datum;
        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"VAL {Datum.Write()}";
    }
}