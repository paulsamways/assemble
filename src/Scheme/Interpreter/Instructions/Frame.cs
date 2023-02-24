namespace Scheme.Interpreter.Instructions;

/// <summary>
/// The <c>Frame</c> instruction creates a new frame from the current environment, the current rib,
/// and <c>Return</c> as the next expression, adds this frame to the current stack, sets the current
/// rib to the empty list, and sets the next expression to <c>Next</c>.
/// </summary>
public class Frame : Instruction
{
    public Frame(Instruction @return, Instruction next)
    {
        Return = @return;
        Next = next;
    }

    public Instruction Return { get; set; }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Frame(Return);
        vm.Next = Next;
    }
}