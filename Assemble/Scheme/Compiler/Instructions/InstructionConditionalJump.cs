namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionConditionalJump : Instruction
{
    public InstructionConditionalJump(int jumpTo, bool predicate)
    {
        JumpTo = jumpTo;
        Predicate = predicate;
    }

    public int JumpTo { get; set; }

    public bool Predicate { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        if (SchemeBoolean.FromObject(accumulator).Value == Predicate)
            interpreter.Next = JumpTo;

        return accumulator;
    }

    public override string ToString()
    {
        return $"IJMP {Predicate} {JumpTo}";
    }
}