namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionNuate : Instruction
{
    public InstructionNuate()
    {
    }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        var result = interpreter.Environment.Get(Scheme.SchemeSymbol.FromString("v")) ?? throw new Exception("missing v");

        interpreter.Nuate(accumulator.To<SchemeContinuation>().Frame);
        interpreter.Return();
        return result;
    }

    public override string ToString()
    {
        return $"NUATE";
    }
}