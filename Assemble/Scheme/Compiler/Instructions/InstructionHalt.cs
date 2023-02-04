namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionHalt : Instruction
{
    public InstructionHalt()
    {
    }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        return accumulator;
    }

    public override string ToString()
    {
        return $"HALT";
    }
}