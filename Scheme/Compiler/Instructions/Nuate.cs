namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Nuate</c> instruction restores <c>Frame</c> to be the current stack, sets the accumulator
/// to the value of <c>Variable</c> in the current environment, and sets the next expression to a
/// <c>Return</c> instruction.
/// </summary>
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
}