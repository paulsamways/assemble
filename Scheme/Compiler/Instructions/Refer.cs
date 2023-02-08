namespace Scheme.Compiler.Instructions;

public class Refer : Instruction
{
    public Refer(string name, Instruction next)
    {
        Name = name;
        Next = next;
    }

    public string Name { get; }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Accumulator =
            vm.Environment.Get(SchemeSymbol.FromString(Name))
                ?? throw new Exception($"Unbound symbol: {Name}");

        vm.Next = Next;
    }

    public override string ToString()
    {
        return $"REF {Name}";
    }
}