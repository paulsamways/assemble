using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class Compiler
{
    private readonly InstructionList instructions;

    public Compiler(InstructionList? instructions = null)
    {
        this.instructions = instructions ?? new InstructionList();
    }

    public void Compile(SchemeDatum o)
    {
        CompileDatum(o);

        instructions.Push(new InstructionHalt());
    }

    private void CompileDatum(SchemeDatum x)
    {
        if (x is SchemeSymbol s)
            CompileReference(s);
        else if (x is SchemePair p)
            CompilePair(p);
        else
            CompileConstant(x);
    }

    private void CompileReference(SchemeSymbol s)
    {
        instructions.Push(new InstructionRefer(s.Value));
    }

    private void CompileConstant(SchemeDatum x)
    {
        instructions.Push(new InstructionConstant(x));
    }

    private void CompilePair(SchemePair p)
    {
        if (p.Car is SchemeSymbol s)
        {
            switch (s.Value)
            {
                case "quote":
                    CompileConstant(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>());
                    return;
                case "set!":
                    CompileAssign(p);
                    return;
                case "if":
                    CompileCondition(p);
                    return;
                case "lambda":
                    CompileClosure(p);
                    return;
                case "call/cc":
                case "call-with-current-continuation":
                    CompileContinuation(p);
                    return;
            }
        }

        // Application
        CompileApplication(p);
    }

    private void CompileAssign(SchemePair p)
    {
        CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>());

        instructions.Push(new InstructionAssign(p.Cdr.To<SchemePair>().Car.To<SchemeSymbol>().Value));
    }

    private void CompileCondition(SchemePair p)
    {
        CompileDatum(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>());

        var jumpWhenFalse = new InstructionConditionalJump(int.MaxValue, false);
        instructions.Push(jumpWhenFalse);

        CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>());
        var jumpPassFalseBranch = new InstructionJump(int.MaxValue);
        instructions.Push(jumpPassFalseBranch);

        jumpWhenFalse.JumpTo = instructions.Next;
        CompileDatum(p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>());

        jumpPassFalseBranch.Index = instructions.Next;
    }

    private void CompileClosure(SchemePair p)
    {
        var closure = new InstructionClosure();
        instructions.Push(closure);

        var jump = new InstructionJump(0);
        instructions.Push(jump);

        closure.Parameters = p.Cdr.To<SchemePair>().Car.To<SchemePair>().AsEnumerable().Select(x => x.To<SchemeSymbol>().Value).ToArray();
        closure.BodyIndex = instructions.Next;
        closure.Body = p;

        foreach (var bodyExpression in p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().AsEnumerable())
            CompileDatum(bodyExpression.To<SchemeDatum>());

        instructions.Push(new InstructionReturn());

        jump.Index = instructions.Next;
    }

    private void CompileApplication(SchemePair p)
    {
        // If the next instruction after this application is a return, then we don't need to push a frame, or return in the closure.

        var frame = new InstructionFrame(0); // New Frame, ReturnTo is the *next* instruction after we execute the procedure
        instructions.Push(frame);

        if (p.Cdr is SchemePair args)
        {
            foreach (var argument in args.AsEnumerable())
            {
                CompileDatum(argument.To<SchemeDatum>());
                instructions.Push(new InstructionArgument());
            }
        }

        // apply the args to the procedure
        CompileDatum(p.Car.To<SchemeDatum>());
        instructions.Push(new InstructionApply());

        frame.ReturnTo = instructions.Next;
    }

    private void CompileContinuation(SchemePair p)
    {
        var ccjump = new InstructionJump(0);
        instructions.Push(ccjump);

        var nuateClosureIndex = instructions.Next;

        var ccclosure = new InstructionClosure();
        ccclosure.Parameters = new string[] { "v" };
        ccclosure.BodyIndex = instructions.Next;

        instructions.Push(new InstructionNuate());
        instructions.Push(new InstructionReturn());

        ccjump.Index = instructions.Next;

        var ccframe = new InstructionFrame(0); // Continuation
        instructions.Push(ccframe);

        instructions.Push(new InstructionConti(nuateClosureIndex));
        instructions.Push(new InstructionArgument());

        CompileDatum(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>());
        instructions.Push(new InstructionApply());

        ccframe.ReturnTo = instructions.Next;
    }
}