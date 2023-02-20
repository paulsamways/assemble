using Scheme.Compiler.Instructions;
using Scheme.Expander;

namespace Scheme.Compiler;

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
            switch (s.Value)
            {
                case "quote":
                    var datum = p.Cdr.To<SchemePair>().Car.To<SchemeDatum>(out SourceInfo? datumSource);

                    return CompileConstant(datum, datumSource, next);
                case "set!":
                    return CompileSet(p, ps, next);
                case "if":
                    return CompileIf(p, ps, next);
                case "lambda":
                    return CompileClosure(p, ps, next);
                case "call/cc":
                case "call-with-current-continuation":
                    return CompileContinuation(p, ps, next);
            }
        }

        // Application
        return CompileApplication(p, ps, next);
    }

    private Instruction CompileSet(SchemePair p, SourceInfo? _, Instruction next)
    {
        var assign = new Assign(p.Cdr.To<SchemePair>().Car.To<SchemeSymbol>().Value, next);

        return CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car, assign);
    }

    private Instruction CompileIf(SchemePair p, SourceInfo? _, Instruction next)
    {
        var then = CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car, next);
        var @else = CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car, next);

        return CompileDatum(p.Cdr.To<SchemePair>().Car, new Test(then, @else));
    }

    private Instruction CompileClosure(SchemePair p, SourceInfo? _, Instruction next)
    {
        var parameters = p.Cdr.To<SchemePair>().Car.To<SchemePair>().ToEnumerable(true).Select(x => x.To<SchemeSymbol>().Value).ToArray();
        Instruction body = new Return();

        foreach (var bodyExpression in p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().ToEnumerable(true).Reverse())
            body = CompileDatum(bodyExpression, body);

        return new Closure(parameters, body, next);
    }

    private Instruction CompileApplication(SchemePair p, SourceInfo? _, Instruction next)
    {
        // apply the args to the procedure
        Instruction proc = CompileDatum(p.Car, new Apply());

        if (p.Cdr is SchemePair args)
        {
            foreach (var argument in args.ToEnumerable(true).Reverse())
                proc = CompileDatum(argument, new Argument(proc));
        }

        if (next is Return)
            return proc;

        return new Instructions.Frame(next, proc);
    }

    private Instruction CompileContinuation(SchemePair p, SourceInfo? _, Instruction next)
    {
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