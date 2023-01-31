namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionAssign : Instruction
{
    public InstructionAssign(string variable)
    {
        Variable = variable;
    }

    public string Variable { get; set; }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        interpreter.Environment.Set(SchemeSymbol.FromString(Variable), accumulator);

        return accumulator;
    }

    public override string ToString()
    {
        return $"ASN";
    }
}