namespace Scheme.Interpreter.Instructions;

/// <summary>
/// The <c>Refer</c> instruction finds the value of the variable <c>name</c> in the current
/// environment, and places this value into the accumulator and sets the next expression to
/// <c>Next</c>.
/// </summary>
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
}