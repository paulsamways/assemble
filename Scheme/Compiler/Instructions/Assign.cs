namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Assign</c> instruction changes the current environment binding for the variable 
/// <c>Variable</c> to the value in the accumulator and sets the next expression to <c>Next</c>.
/// </summary>
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
}