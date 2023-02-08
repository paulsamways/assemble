namespace Scheme.Compiler.Instructions;

public class Constant : Instruction
{
    public Constant(SchemeDatum datum, Instruction next)
    {
        Datum = datum;
        Next = next;
    }

    public SchemeDatum Datum { get; }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Accumulator = Datum;
        vm.Next = Next;
    }

    public override string ToString() => $"VAL {Datum}";
}