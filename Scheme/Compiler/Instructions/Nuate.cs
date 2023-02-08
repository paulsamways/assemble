namespace Scheme.Compiler.Instructions;

public class Nuate : Instruction
{
    public Nuate(Scheme.Compiler.Frame frame, string variable)
    {
        Frame = frame;
        Variable = variable;
    }

    public Scheme.Compiler.Frame Frame { get; set; }

    public string Variable { get; set; }

    public override void Execute(VM vm)
    {
        vm.Accumulator = vm.Environment.Get(SchemeSymbol.FromString(Variable)) ?? throw new Exception($"missing {Variable}");
        vm.Next = new Return();
        vm.Nuate(Frame);
    }

    public override string ToString()
    {
        return $"NUATE";
    }
}