namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionJump : Instruction
{
    public InstructionJump(int index)
    {
        Index = index;
    }

    public int Index { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        interpreter.Next = Index;

        return accumulator;
    }

    public override string ToString()
    {
        return $"JMP {Index}";
    }
}