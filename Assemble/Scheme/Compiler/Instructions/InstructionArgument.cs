namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionArgument : Instruction
{
    public InstructionArgument(Instruction next)
    {
        Next = next;
    }

    public Instruction Next { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Ribs.Add(interpreter.Accumulator);
        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"ARG";
    }
}