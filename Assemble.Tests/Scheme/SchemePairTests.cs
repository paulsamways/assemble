namespace Assemble.Scheme;

public class SchemePairTests
{
    [Fact]
    public void FromEnumerable_EmptyEnumerationReturnsEmptyList()
    {
        var x = SchemePair.FromEnumerable(Array.Empty<SchemeDatum>());

        Assert.IsType<SchemeEmptyList>(x);
    }

    [Fact]
    public void FromEnumerable_ReturnsEmptyListTerminatedPairLinkedList()
    {
        var xs = SchemePair.FromEnumerable(new SchemeDatum[]{
            new SchemeNumber(1),
            new SchemeNumber(2),
            new SchemeNumber(3)
        });

        var p1 = xs.To<SchemePair>();
        var p2 = xs.To<SchemePair>().Cdr.To<SchemePair>();
        var p3 = xs.To<SchemePair>().Cdr.To<SchemePair>().Cdr.To<SchemePair>();

        Assert.Equal(1, p1.Car.To<SchemeNumber>().Value);
        Assert.Equal(2, p2.Car.To<SchemeNumber>().Value);
        Assert.Equal(3, p3.Car.To<SchemeNumber>().Value);

        Assert.Equal(SchemeEmptyList.Value, p3.Cdr);
    }
}