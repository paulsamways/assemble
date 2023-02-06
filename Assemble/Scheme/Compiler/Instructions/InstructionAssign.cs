namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionAssign : Instruction
{
    public InstructionAssign(string variable, Instruction next)
    {
        Variable = variable;
        Next = next;
    }

    public string Variable { get; set; }

    public Instruction Next { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Environment.Set(SchemeSymbol.FromString(Variable), interpreter.Accumulator);
        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"ASN";
    }
}