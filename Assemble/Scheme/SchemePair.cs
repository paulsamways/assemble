namespace Assemble.Scheme;

public sealed class SchemePair : SchemeDatum
{
    public SchemePair(SchemeObject car, SchemeObject cdr)
    {
        Car = car;
        Cdr = cdr;
    }

    public SchemeObject Car { get; set; }

    public SchemeObject Cdr { get; set; }

    public bool IsList => ToEnumerable().Last() is SchemeEmptyList;

    public bool IsImproperList => ToEnumerable().Last() is not SchemeEmptyList;

    public override string Name => "pair";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemePair p && p.Car.Equals(Car) && p.Cdr.Equals(Cdr);

    public override string Write()
    {
        var items = ToEnumerable().ToArray();
        if (items[^1] is SchemeEmptyList)
            return "(" + string.Join(" ", items[0..^1].Select(x => x.Write())) + ")";

        return $"({Car.Write()} . {Cdr.Write()})";
    }

    public IEnumerable<SchemeObject> ToEnumerable(bool asList = false)
    {
        yield return Car;

        if (Cdr is SchemePair p)
        {
            foreach (var cdr in p.ToEnumerable())
                yield return cdr;
        }
        else
        {
            if (asList)
            {
                if (Cdr is not SchemeEmptyList)
                    throw new Exception("Pair was enumerated as a list, but is improper");

                yield break;
            }

            yield return Cdr;
        }
    }

    public static SchemeObject FromEnumerable(IEnumerable<SchemeObject> values)
    {
        return
            values
            .Reverse()
            .Aggregate((SchemeDatum)SchemeEmptyList.Value, (cdr, value) => new SchemePair(value, cdr));
    }
}
