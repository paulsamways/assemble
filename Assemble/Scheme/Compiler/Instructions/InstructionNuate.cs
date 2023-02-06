namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionNuate : Instruction
{
    public InstructionNuate(Frame frame, string variable)
    {
        Frame = frame;
        Variable = variable;
    }

    public Frame Frame { get; set; }

    public string Variable { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Accumulator = interpreter.Environment.Get(SchemeSymbol.FromString(Variable)) ?? throw new Exception($"missing {Variable}");
        interpreter.Next = new InstructionReturn();
        interpreter.Nuate(Frame);

        // interpreter.Nuate(accumulator.To<SchemeContinuation>().Frame);
        // interpreter.Return();

        // return result;
    }

    public override string ToString()
    {
        return $"NUATE";
    }
}