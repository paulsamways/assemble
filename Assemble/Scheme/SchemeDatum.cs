using Assemble.Scheme.Compiler;
using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme;

public abstract class SchemeDatum : SchemeObject
{
    public virtual void Compile(InstructionList instructions)
    {
        instructions.Push(new InstructionConstant(this));
    }
}