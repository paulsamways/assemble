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

    public bool IsList => Cdr is SchemeEmptyList || (Cdr is SchemePair p && p.IsList);

    public bool IsImproperList => Cdr is not SchemePair || (Cdr is SchemePair p && p.IsImproperList);

    public override string Name => "pair";

    public override bool Equals(SchemeDatum? other)
    {
        return other is not null && other is SchemePair p && p.Car.Equals(Car) && p.Cdr.Equals(Cdr);
    }

    public override string Write()
    {
        if (IsList)
            return $"({string.Join(' ', AsEnumerable().Select(x => x.Write()))})";

        return $"({Car.Write()} . {Cdr.Write()})";
    }

    public override SchemeObject Evaluate(Environment e)
    {
        return Car.Evaluate(e).To<SchemeCallable>().Call(e, Cdr);
    }

    public IEnumerable<SchemeObject> AsEnumerable()
    {
        yield return Car;

        if (Cdr is SchemePair p)
        {
            foreach (var cdr in p.AsEnumerable())
                yield return cdr;
        }
        else if (Cdr is SchemeEmptyList)
        {
            yield break;
        }
        else
        {
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
