namespace Scheme.Compiler.Instructions;

public class Halt : Instruction
{
    public override void Execute(VM vm)
    {
        vm.Next = null;
    }

    public override string ToString()
    {
        return $"HALT";
    }
}