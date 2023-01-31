namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionReturn : Instruction
{
    public InstructionReturn()
    {
    }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        interpreter.Return();

        return accumulator;
    }

    public override string ToString()
    {
        return $"RET";
    }
}