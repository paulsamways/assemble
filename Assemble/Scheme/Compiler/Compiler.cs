using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class Compiler
{
    private readonly InstructionList instructions;

    public Compiler(InstructionList? instructions = null)
    {
        this.instructions = instructions ?? new InstructionList();
    }

    public Instruction Compile(SchemeDatum o)
    {
        return CompileDatum(o, new InstructionHalt());
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
        return new InstructionRefer(s.Value, next);
    }

    private Instruction CompileConstant(SchemeDatum x, Instruction next)
    {
        return new InstructionConstant(x, next);
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
        var assign = new InstructionAssign(p.Cdr.To<SchemePair>().Car.To<SchemeSymbol>().Value, next);

        return CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>(), assign);
    }

    private Instruction CompileIf(SchemePair p, Instruction next)
    {
        var then = CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>(), next);
        var @else = CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>(), next);

        return CompileDatum(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>(), new InstructionTest(then, @else));
    }

    private Instruction CompileClosure(SchemePair p, Instruction next)
    {
        var parameters = p.Cdr.To<SchemePair>().Car.To<SchemePair>().AsEnumerable().Select(x => x.To<SchemeSymbol>().Value).ToArray();
        Instruction body = new InstructionReturn();

        foreach (var bodyExpression in p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().AsEnumerable().Reverse())
            body = CompileDatum(bodyExpression.To<SchemeDatum>(), body);

        return new InstructionClosure(parameters, body, next);
    }

    private Instruction CompileApplication(SchemePair p, Instruction next)
    {
        // apply the args to the procedure
        Instruction proc = CompileDatum(p.Car.To<SchemeDatum>(), new InstructionApply());

        if (p.Cdr is SchemePair args)
        {
            foreach (var argument in args.AsEnumerable().Reverse())
                proc = CompileDatum(argument.To<SchemeDatum>(), new InstructionArgument(proc));
        }

        if (next is InstructionReturn)
            return proc;

        return new InstructionFrame(next, proc);
    }

    private Instruction CompileContinuation(SchemePair p, Instruction next)
    {
        var conti = new InstructionConti(
            new InstructionArgument(
                CompileDatum(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>(),
                    new InstructionApply()
                )
            )
        );

        if (next is InstructionReturn)
            return conti;

        return new InstructionFrame(next, conti);
    }
}