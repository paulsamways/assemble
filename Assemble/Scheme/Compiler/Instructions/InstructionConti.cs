namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionConti : Instruction
{
    public InstructionConti(Instruction next)
    {
        Next = next;
    }

    public Instruction Next { get; set; }

    public override void Execute(Interpreter interpreter)
    {
        interpreter.Accumulator = new SchemeProcedure(new Environment(), new string[] { "v" }, new InstructionNuate(interpreter.StackFrame!, "v"));

        interpreter.Next = Next;
    }

    public override string ToString()
    {
        return $"CONTI";
    }
}