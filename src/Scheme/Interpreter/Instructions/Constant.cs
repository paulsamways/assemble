namespace Scheme.Interpreter.Instructions;

/// <summary>
/// The <c>Constant</c> instruction places <c>Datum</c> into the accumulator and 
/// sets the next expression to <c>Next</c>.
/// </summary>
public class Constant : Instruction
{
    public Constant(SchemeObject datum, Instruction next)
    {
        Datum = datum;
        Next = next;
    }

    public SchemeObject Datum { get; }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Accumulator = Datum;
        vm.Next = Next;
    }
}