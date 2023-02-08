namespace Scheme.Compiler.Instructions;

public class Assign : Instruction
{
    public Assign(string variable, Instruction next)
    {
        Variable = variable;
        Next = next;
    }

    public string Variable { get; set; }

    public Instruction Next { get; set; }

    public override void Execute(VM vm)
    {
        vm.Environment.Set(SchemeSymbol.FromString(Variable), vm.Accumulator);
        vm.Next = Next;
    }

    public override string ToString()
    {
        return $"ASN";
    }
}