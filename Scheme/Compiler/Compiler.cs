using Scheme.Compiler.Instructions;

namespace Scheme.Compiler;

public class Compiler
{
    public Instruction Compile(SchemeDatum o)
    {
        return CompileDatum(o, new Halt());
    }

    private Instruction CompileDatum(SchemeDatum x, Instruction next)
    {
        if (x is SchemeSymbol s)
            return CompileReference(s, next);
        else if (x is SchemePair p)
            return CompilePair(p, next);
        else
            return CompileConstant(x, next);
    }

    private Instruction CompileReference(SchemeSymbol s, Instruction next)
    {
        return new Refer(s.Value, next);
    }

    private Instruction CompileConstant(SchemeDatum x, Instruction next)
    {
        return new Constant(x, next);
    }

    private Instruction CompilePair(SchemePair p, Instruction next)
    {
        if (p.Car is SchemeSymbol s)
        {
            switch (s.Value)
            {
                case "quote":
                    return CompileConstant(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>(), next);
                case "set!":
                    return CompileSet(p, next);
                case "if":
                    return CompileIf(p, next);
                case "lambda":
                    return CompileClosure(p, next);
                case "call/cc":
                case "call-with-current-continuation":
                    return CompileContinuation(p, next);
            }
        }

        // Application
        return CompileApplication(p, next);
    }

    private Instruction CompileSet(SchemePair p, Instruction next)
    {
        var assign = new Assign(p.Cdr.To<SchemePair>().Car.To<SchemeSymbol>().Value, next);

        return CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>(), assign);
    }

    private Instruction CompileIf(SchemePair p, Instruction next)
    {
        var then = CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>(), next);
        var @else = CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>(), next);

        return CompileDatum(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>(), new Test(then, @else));
    }

    private Instruction CompileClosure(SchemePair p, Instruction next)
    {
        var parameters = p.Cdr.To<SchemePair>().Car.To<SchemePair>().ToEnumerable(true).Select(x => x.To<SchemeSymbol>().Value).ToArray();
        Instruction body = new Return();

        foreach (var bodyExpression in p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().ToEnumerable(true).Reverse())
            body = CompileDatum(bodyExpression.To<SchemeDatum>(), body);

        return new Closure(parameters, body, next);
    }

    private Instruction CompileApplication(SchemePair p, Instruction next)
    {
        // apply the args to the procedure
        Instruction proc = CompileDatum(p.Car.To<SchemeDatum>(), new Apply());

        if (p.Cdr is SchemePair args)
        {
            foreach (var argument in args.ToEnumerable(true).Reverse())
                proc = CompileDatum(argument.To<SchemeDatum>(), new Argument(proc));
        }

        if (next is Return)
            return proc;

        return new Instructions.Frame(next, proc);
    }

    private Instruction CompileContinuation(SchemePair p, Instruction next)
    {
        var conti = new Conti(
            new Argument(
                CompileDatum(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>(),
                    new Apply()
                )
            )
        );

        if (next is Return)
            return conti;

        return new Instructions.Frame(next, conti);
    }
}