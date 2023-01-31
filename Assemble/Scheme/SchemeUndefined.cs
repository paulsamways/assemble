using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme;

public sealed class SchemeUndefined : SchemeObject
{
    private SchemeUndefined()
    {
    }

    public static readonly SchemeUndefined Value = new();

    public override string Name => "undefined";

    public override SchemeObject Evaluate(Environment e) => this;

    public override bool Equals(SchemeObject? other) => false;
    public override bool Same(SchemeObject other) => false;
}