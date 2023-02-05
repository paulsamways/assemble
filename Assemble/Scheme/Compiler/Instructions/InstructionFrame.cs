namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionFrame : Instruction
{
    public InstructionFrame(int returnTo)
    {
        ReturnTo = returnTo;
    }

    public int ReturnTo { get; set; }

    private static int FrameCount = 0;

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        interpreter.Frame(ReturnTo);

        System.Console.WriteLine("Frame #{0}", FrameCount++);

        return accumulator;
    }

    public override string ToString()
    {
        return $"FRAME {ReturnTo}";
    }
}