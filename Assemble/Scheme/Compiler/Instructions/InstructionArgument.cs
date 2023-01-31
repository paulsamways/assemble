namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionArgument : Instruction
{
    public InstructionArgument()
    {
    }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        interpreter.Ribs.Add(accumulator);

        return accumulator;
    }

    public override string ToString()
    {
        return $"ARG";
    }
}