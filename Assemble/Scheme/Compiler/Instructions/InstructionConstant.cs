namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionConstant : Instruction
{
    public InstructionConstant(SchemeDatum datum)
    {
        Datum = datum;
    }

    public SchemeDatum Datum { get; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        return Datum;
    }

    public override string ToString()
    {
        return $"VAL {Datum.Write()}";
    }
}