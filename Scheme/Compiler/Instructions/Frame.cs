namespace Scheme.Compiler.Instructions;

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