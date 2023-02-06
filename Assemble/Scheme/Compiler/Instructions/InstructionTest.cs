namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionTest : Instruction
{
    public InstructionTest(Instruction then, Instruction @else)
    {
        Then = then;
        Else = @else;
    }

    public Instruction Then { get; set; }

    public Instruction Else { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Next = (interpreter.Accumulator is SchemeBoolean b && b.Value)
            ? Then
            : Else;
    }

    public override string ToString()
    {
        return $"TEST";
    }
}