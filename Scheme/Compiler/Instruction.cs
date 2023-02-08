namespace Scheme.Compiler;

public abstract class Instruction
{
    public abstract void Execute(VM vm);

    public abstract override string ToString();
}