using Scheme.Compiler;

namespace Scheme.Tests.Compiler;

public class BindingTableTests
{
    private readonly BindingTable _bindings;
    private readonly Scope _sc1 = new();
    private readonly Scope _sc2 = new();
    private readonly SchemeSymbol _locA = SchemeSymbol.Gensym("a");
    private readonly SchemeSymbol _locBOut = SchemeSymbol.Gensym("b");
    private readonly SchemeSymbol _locBIn = SchemeSymbol.Gensym("b");
    private readonly SchemeSymbol _locC1 = SchemeSymbol.Gensym("c");
    private readonly SchemeSymbol _locC2 = SchemeSymbol.Gensym("c");

    public BindingTableTests()
    {
        _bindings = new();
        _bindings.Add(new SchemeSyntaxObject(SchemeSymbol.FromString("a"), _sc1), _locA);
        _bindings.Add(new SchemeSyntaxObject(SchemeSymbol.FromString("b"), _sc1), _locBOut);
        _bindings.Add(new SchemeSyntaxObject(SchemeSymbol.FromString("b"), _sc1, _sc2), _locBIn);

        _bindings.Add(new SchemeSyntaxObject(SchemeSymbol.FromString("c"), _sc1), _locC1);
        _bindings.Add(new SchemeSyntaxObject(SchemeSymbol.FromString("c"), _sc2), _locC2);
    }

    private void ResolvesTo(SchemeSyntaxObject o, SchemeSymbol s)
    {
        Assert.True(_bindings.TryResolve(o, out SchemeSymbol? s2));
        Assert.Equal(s, s2);
    }

    private void DoesNotResolve(SchemeSyntaxObject o)
    {
        Assert.False(_bindings.TryResolve(o, out SchemeSymbol? _));
    }

    [Fact]
    public void Resolve_Simple()
    {
        ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("a"), _sc1), _locA);
        ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("a"), _sc1, _sc2), _locA);
        DoesNotResolve(new SchemeSyntaxObject(SchemeSymbol.FromString("a"), _sc2));

        ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("b"), _sc1), _locBOut);
        ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("b"), _sc1, _sc2), _locBIn);
        DoesNotResolve(new SchemeSyntaxObject(SchemeSymbol.FromString("b"), _sc2));

        ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("c"), _sc1), _locC1);
        Assert.Throws<Exception>(() => ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("c"), _sc1, _sc2), _locC1));
        ResolvesTo(new SchemeSyntaxObject(SchemeSymbol.FromString("c"), _sc2), _locC2);
    }
}
