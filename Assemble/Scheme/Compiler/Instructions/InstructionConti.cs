namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionConti : Instruction
{
    public InstructionConti(int nuateBodyIndex)
    {
        NuateBodyIndex = nuateBodyIndex;
    }

    public int NuateBodyIndex { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        return new SchemeContinuation(interpreter.StackFrame!, new Environment(interpreter.Environment), new string[] { "v" })
        {
            BodyIndex = NuateBodyIndex
        };
    }

    public override string ToString()
    {
        return $"CONTI {NuateBodyIndex}";
    }
}