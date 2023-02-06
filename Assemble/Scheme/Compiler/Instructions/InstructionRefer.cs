namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionRefer : Instruction
{
    public InstructionRefer(string name, Instruction next)
    {
        Name = name;
        Next = next;
    }

    public string Name { get; }

    public Instruction Next { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Accumulator =
            interpreter.Environment.Get(SchemeSymbol.FromString(Name))
                ?? throw new Exception($"Unbound symbol: {Name}");

        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"REF {Name}";
    }
}