namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Halt</c> instruction halts the virtual machine. The value in the accumulator is the result of the computation.
/// </summary>
public class Halt : Instruction
{
    public override void Execute(VM vm)
    {
        vm.Next = null;
    }
}