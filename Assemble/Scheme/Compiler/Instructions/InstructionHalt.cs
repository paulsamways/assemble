namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionHalt : Instruction
{
    public override void Execute(Interpreter interpreter)
    {
        interpreter.Next = null;
    }

    public override string ToString()
    {
        return $"HALT";
    }
}