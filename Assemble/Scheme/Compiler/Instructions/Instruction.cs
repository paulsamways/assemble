namespace Assemble.Scheme.Compiler.Instructions;

public abstract class Instruction
{
    public abstract override string ToString();

    public abstract SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter);
}