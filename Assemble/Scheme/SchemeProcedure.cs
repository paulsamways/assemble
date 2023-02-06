using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme;

public class SchemeProcedure : SchemeObject
{
    public SchemeProcedure(Environment closure, string[] parameters, Instruction body)
    {
        Closure = closure;
        Parameters = parameters;
        Body = body;
    }

    public Environment Closure { get; init; }

    public string[] Parameters { get; init; }

    public Instruction Body { get; init; }

    public override string Name => "procedure";


    public override bool Equals(SchemeObject? other)
        => false;
}