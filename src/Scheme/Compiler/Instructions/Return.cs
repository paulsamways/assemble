namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Return</c> instruction removes the first frame from the stack and resets the current
/// environment, the current rib, the next expression, and the current stack.
/// </summary>
public class Return : Instruction
{
    public override void Execute(VM vm)
    {
        vm.Return();
    }
}