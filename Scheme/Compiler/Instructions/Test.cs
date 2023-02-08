namespace Scheme.Compiler.Instructions;

public class Test : Instruction
{
    public Test(Instruction then, Instruction @else)
    {
        Then = then;
        Else = @else;
    }

    public Instruction Then { get; set; }

    public Instruction Else { get; set; }

    public override void Execute(VM vm)
    {
        vm.Next = (vm.Accumulator is SchemeBoolean b && b.Value)
            ? Then
            : Else;
    }

    public override string ToString()
    {
        return $"TEST";
    }
}