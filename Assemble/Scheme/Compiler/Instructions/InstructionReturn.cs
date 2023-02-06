namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionReturn : Instruction
{
    public override void Execute(Interpreter interpreter)
    {
        interpreter.Return();
    }

    public override string ToString()
    {
        return $"RET";
    }
}