namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionFrame : Instruction
{
    public InstructionFrame(Instruction @return, Instruction next)
    {
        Return = @return;
        Next = next;
    }

    public Instruction Return { get; set; }

    public Instruction Next { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Frame(Return);
        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"FRAME";
    }
}