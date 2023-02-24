using Scheme.Interpreter.Instructions;
using static Scheme.Match;

namespace Scheme.Interpreter;

public class Compiler
{
    public Instruction Compile(SchemeObject o)
    {
        return CompileDatum(o, new Halt());
    }

    private Instruction CompileDatum(SchemeObject o, Instruction next)
    {
        if (o.TryTo(out SchemeSymbol? s, out SourceInfo? ss))
            return CompileReference(s, ss, next);
        else if (o.TryTo(out SchemePair? p, out SourceInfo? ps))
            return CompilePair(p, ps, next);
        else if (o.TryTo(out SchemeDatum? d, out SourceInfo? ds))
            return CompileConstant(d, ds, next);

        throw new Exception("can not compile " + o.Name);
    }

    private Instruction CompileReference(SchemeSymbol s, SourceInfo? _, Instruction next)
    {
        return new Refer(s.Value, next);
    }

    private Instruction CompileConstant(SchemeDatum x, SourceInfo? _, Instruction next)
    {
        return new Constant(x, next);
    }

    private Instruction CompilePair(SchemePair p, SourceInfo? ps, Instruction next)
    {
        if (p.Car.TryTo(out SchemeSymbol? s, out SourceInfo? _))
        {
            if (s.Equals(SchemeSymbol.Form.Quote))
                return CompileQuote(p, ps, next);
            else if (s.Equals(SchemeSymbol.Form.Set))
                return CompileSet(p, ps, next);
            else if (s.Equals(SchemeSymbol.Form.If))
                return CompileIf(p, ps, next);
            else if (s.Equals(SchemeSymbol.Form.Lambda))
                return CompileClosure(p, ps, next);
            else if (s.Equals(SchemeSymbol.Form.CallWithCurrentContinuation) || s.Equals(SchemeSymbol.Form.CallCC))
                return CompileContinuation(p, ps, next);
        }

        return CompileApplication(p, ps, next);
    }

    private Instruction CompileQuote(SchemePair p, SourceInfo? _, Instruction next)
    {
        var (_, _, datum) = MatchOrThrow(p,
            List(Symbol(SchemeSymbol.Form.Quote), AnyDatum));

        return CompileConstant(datum, null, next);
    }

    private Instruction CompileSet(SchemePair p, SourceInfo? _, Instruction next)
    {
        var (_, _, name, value) = MatchOrThrow(p,
            List(Symbol(SchemeSymbol.Form.Set), AnySymbol, AnyDatum));

        return CompileDatum(value, new Assign(name.Value, next));
    }

    private Instruction CompileIf(SchemePair p, SourceInfo? _, Instruction next)
    {
        var (_, _, predicate, then, @else) = MatchOrThrow(p,
            List(Symbol(SchemeSymbol.Form.If), AnyDatum, AnyDatum, AnyDatum));

        return CompileDatum(predicate,
            new Test(CompileDatum(then, next), CompileDatum(@else, next)));
    }

    private Instruction CompileClosure(SchemePair p, SourceInfo? _, Instruction next)
    {
        var (_, _, (_, parameters), expressions) = MatchOrThrow(p,
            ListMany(Symbol(SchemeSymbol.Form.Lambda), ListMany(AnySymbol), AnyDatum));

        Instruction body = new Return();

        foreach (var bodyExpression in expressions.Reverse())
            body = CompileDatum(bodyExpression, body);

        return new Closure(parameters.Select(x => x.Value).ToArray(), body, next);
    }

    private Instruction CompileApplication(SchemePair p, SourceInfo? _, Instruction next)
    {
        var (_, binding, arguments) = MatchOrThrow(p, ListMany(AnyDatum, AnyDatum));

        Instruction proc = CompileDatum(binding, new Apply());

        foreach (var argument in arguments.Reverse())
            proc = CompileDatum(argument, new Argument(proc));

        if (next is Return)
            return proc;

        return new Instructions.Frame(next, proc);
    }

    private Instruction CompileContinuation(SchemePair p, SourceInfo? _, Instruction next)
    {
        // TODO: Use Matcher
        var conti = new Conti(
            new Argument(
                CompileDatum(p.Cdr.To<SchemePair>().Car,
                    new Apply()
                )
            )
        );

        if (next is Return)
            return conti;

        return new Instructions.Frame(next, conti);
    }
}