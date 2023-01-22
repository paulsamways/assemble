namespace Assemble.Scheme;

public sealed class SchemePair : SchemeDatum
{
    public SchemePair(SchemeDatum car, SchemeDatum cdr)
    {
        Car = car;
        Cdr = cdr;
    }

    public SchemeDatum Car { get; init; }

    public SchemeDatum Cdr { get; init; }

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemePair p && p.Car.Equals(Car) && p.Cdr.Equals(Cdr);
    }

    public override string Print()
    {
        return $"({Car.Print()} . {Cdr.Print()})";
    }

    public static SchemeDatum FromEnumerable(IEnumerable<SchemeDatum> values)
    {
        return
            values
            .Reverse()
            .Aggregate((SchemeDatum)SchemeEmptyList.Value, (cdr, value) => new SchemePair(value, cdr));
    }
}
