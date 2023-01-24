namespace Assemble.Scheme;

public sealed class SchemeBoolean : SchemeDatum, Wraps<SchemeBoolean, bool>
{
    private SchemeBoolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; init; }

    public override string Name => "boolean";

    public static SchemeBoolean FromBoolean(bool value)
    {
        return value ? True : False;
    }

    public static SchemeBoolean FromObject(SchemeObject o)
    {
        return o is SchemeBoolean b && !b.Value
            ? False : True;
    }

    public override string Write()
    {
        return Value ? "#t" : "#f";
    }

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeBoolean b && b.Value == Value;

    public override bool Same(SchemeObject other)
        => Equals(other);

    public override SchemeObject Evaluate(Environment e)
    {
        return this;
    }

    public bool Unwrap()
    {
        return Value;
    }

    public static SchemeBoolean Wrap(bool value)
    {
        return FromBoolean(value);
    }

    public static readonly SchemeBoolean True = new(true);
    public static readonly SchemeBoolean False = new(false);
}