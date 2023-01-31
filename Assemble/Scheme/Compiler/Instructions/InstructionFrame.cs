namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionFrame : Instruction
{
    public InstructionFrame(int next)
    {
        Next = next;
    }

    public int Next { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        interpreter.Frame(Next);

        return accumulator;
    }

    public override string ToString()
    {
        return $"FRAME {Next}";
    }
}