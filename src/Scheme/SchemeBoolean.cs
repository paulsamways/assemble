namespace Scheme;

public sealed class SchemeBoolean : SchemeDatum, Wraps<SchemeBoolean, bool>
{
    private SchemeBoolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; init; }

    public override string Name => "boolean";

    public override bool Same(SchemeObject other)
        => Equals(other);

    public static SchemeBoolean Wrap(bool value)
        => FromBoolean(value);

    public bool Unwrap() => Value;

    public override string ToString() => Value ? "#t" : "#f";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeBoolean b && b.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static SchemeBoolean FromBoolean(bool value) =>
         value ? True : False;

    public static SchemeBoolean FromObject(SchemeObject o) =>
         o is SchemeBoolean b && !b.Value
            ? False : True;

    public static readonly SchemeBoolean True = new(true);
    public static readonly SchemeBoolean False = new(false);
}