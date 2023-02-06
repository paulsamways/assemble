using Assemble.Scheme.Compiler;
using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme;

public class SchemeContinuation : SchemeProcedure
{
    public SchemeContinuation(Frame frame, Environment e, string[] parameters, Instruction body)
        : base(e, parameters, body)
    {
        Frame = frame;
    }

    public Frame Frame { get; set; }

    public override string Name => "continuation";

    public override bool Equals(SchemeObject? other)
        => false;

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }
}