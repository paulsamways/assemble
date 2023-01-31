namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionRefer : Instruction
{
    public InstructionRefer(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        return interpreter.Environment.Get(SchemeSymbol.FromString(Name))
            ?? throw new Exception($"Unbound symbol: {Name}");
    }

    public override string ToString()
    {
        return $"REF {Name}";
    }
}