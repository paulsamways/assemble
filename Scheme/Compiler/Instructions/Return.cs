namespace Scheme.Compiler.Instructions;

public class Return : Instruction
{
    public override void Execute(VM vm)
    {
        vm.Return();
    }
}