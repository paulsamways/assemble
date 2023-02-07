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

    [Fact]
    public void ToEnumerable_YieldsEmptyListWhenProperList()
    {
        var x = new SchemePair(
            new SchemeNumber(1),
            new SchemePair(
                new SchemeNumber(2),
                new SchemePair(
                    new SchemeNumber(3),
                    SchemeEmptyList.Value
                )
            )
        );

        var values = x.ToEnumerable().ToArray();
        Assert.Equal(1, values[0].To<SchemeNumber>().Value);
        Assert.Equal(2, values[1].To<SchemeNumber>().Value);
        Assert.Equal(3, values[2].To<SchemeNumber>().Value);
        Assert.IsType<SchemeEmptyList>(values[3]);
    }

    [Fact]
    public void IsImproperList_TrueWhenImproper()
    {
        var x = new SchemePair(
            new SchemeNumber(1),
            new SchemePair(
                new SchemeNumber(2),
                new SchemeNumber(3)
            )
        );

        Assert.True(x.IsImproperList);
    }

    [Fact]
    public void IsImproperList_FalseWhenNotImproper()
    {
        var x = new SchemePair(
            new SchemeNumber(1),
            new SchemePair(
                new SchemeNumber(2),
                SchemeEmptyList.Value
            )
        );

        Assert.False(x.IsImproperList);
    }
}