namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionConditionalJump : Instruction
{
    public InstructionConditionalJump(int index, bool predicate)
    {
        Index = index;
        Predicate = predicate;
    }

    public int Index { get; set; }

    public bool Predicate { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        if (SchemeBoolean.FromObject(accumulator).Value == Predicate)
            interpreter.Next = Index;

        return accumulator;
    }

    public override string ToString()
    {
        return $"IJMP {Predicate} {Index}";
    }
}