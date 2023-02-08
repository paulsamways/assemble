using Scheme.Compiler.Instructions;

namespace Scheme;

public sealed class SchemeString : SchemeDatum
{
    public SchemeString(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

    public override string Name => "string";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeString b && b.Value.Equals(Value);

    public override string Write()
    {
        // TODO: Escape specials
        return $"\"{Value}\"";
    }
}