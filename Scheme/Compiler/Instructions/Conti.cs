namespace Scheme.Compiler.Instructions;

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

    public override string ToString()
    {
        return $"CONTI";
    }
}