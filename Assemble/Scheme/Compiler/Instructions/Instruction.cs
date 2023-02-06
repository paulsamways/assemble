namespace Assemble.Scheme.Compiler.Instructions;

public abstract class Instruction
{
    public abstract void Execute(Interpreter interpreter);

    public abstract override string ToString();
}