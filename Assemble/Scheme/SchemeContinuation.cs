using Assemble.Scheme.Compiler;

namespace Assemble.Scheme;

public class SchemeContinuation : SchemeProcedure
{
    public SchemeContinuation(Frame frame, Environment e, string[] parameters)
        : base(e, parameters, Array.Empty<SchemeObject>())
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