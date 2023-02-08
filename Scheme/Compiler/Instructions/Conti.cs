namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Conti</c> instruction creates a continuation from the current stack, places this 
/// continuation in the accumulator, and sets the next expression to <c>Next</c>.
/// </summary>
public class Conti : Instruction
{
    public Conti(Instruction next)
    {
        Next = next;
    }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Accumulator = new SchemeProcedure(new Environment(), new string[] { "v" }, new Nuate(vm.StackFrame!, "v"));

        vm.Next = Next;
    }
}