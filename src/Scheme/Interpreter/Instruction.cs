namespace Scheme.Interpreter;

public abstract class Instruction
{
    public abstract void Execute(VM vm);
}