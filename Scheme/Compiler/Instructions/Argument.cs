namespace Scheme.Compiler.Instructions;

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

    public override string ToString()
    {
        return $"ARG";
    }
}