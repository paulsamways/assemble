namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionClosure : Instruction
{
    public InstructionClosure()
    {
        Parameters = Array.Empty<string>();
    }

    public string[] Parameters { get; set; }

    public int BodyIndex { get; set; }

    public SchemeDatum? Body { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        return new SchemeProcedure(interpreter.Environment, Parameters, Array.Empty<SchemeObject>())
        {
            BodyIndex = BodyIndex
        };
    }

    public override string ToString()
    {
        return $"CLSR {BodyIndex} - {Body?.Write()}";
    }
}