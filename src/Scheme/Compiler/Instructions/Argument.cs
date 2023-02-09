namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Argument</c> instruction adds the value in the accumulator to the current rib and sets the next expression to <c>Next</c>.
/// </summary>
public class Argument : Instruction
{
    public Argument(Instruction next)
    {
        Next = next;
    }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Ribs.Add(vm.Accumulator);
        vm.Next = Next;
    }
}