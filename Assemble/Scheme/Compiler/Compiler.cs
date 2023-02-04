using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class Compiler
{
    private readonly InstructionList instructions;

    public Compiler(InstructionList? instructions = null)
    {
        this.instructions = instructions ?? new InstructionList();
    }

    public void Compile(SchemeDatum o) => Compile(o, new InstructionHalt());

    private void Compile(SchemeDatum x, Instruction next)
    {
        if (x is SchemeSymbol s)
            CompileReference(s, next);
        else if (x is SchemePair p)
            CompilePair(p, next);
        else
            CompileConstant(x, next);
    }

    private void CompileReference(SchemeSymbol s, Instruction next)
    {
        instructions.Push(new InstructionRefer(s.Value));
        instructions.Push(next);
    }

    private void CompileConstant(SchemeDatum x, Instruction next)
    {
        instructions.Push(new InstructionConstant(x));
        instructions.Push(next);
    }

    private void CompilePair(SchemePair p, Instruction next)
    {
        if (p.Car is SchemeSymbol s)
        {
            switch (s.Value)
            {
                case "quote":
                    instructions.Push(new InstructionConstant(p.Cdr.To<SchemePair>().Car.To<SchemeDatum>()));
                    break;
                case "set!":
                    p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>().Compile(instructions);

                    instructions.Push(new InstructionAssign(p.Cdr.To<SchemePair>().Car.To<SchemeSymbol>().Value));
                    break;
                case "if":

                    var jif = new InstructionConditionalJump(int.MaxValue, false);
                    var j = new InstructionJump(int.MaxValue);

                    var cdr = p.Cdr.To<SchemePair>();
                    cdr.Car.To<SchemeDatum>().Compile(instructions);
                    cdr = cdr.Cdr.To<SchemePair>();
                    instructions.Push(jif);
                    cdr.Car.To<SchemeDatum>().Compile(instructions);
                    instructions.Push(j);
                    jif.Index = instructions.Next;
                    cdr.Cdr.To<SchemePair>().Car.To<SchemeDatum>().Compile(instructions);
                    j.Index = instructions.Next;
                    break;
                case "lambda":

                    var jump = new InstructionJump(0);
                    instructions.Push(jump);

                    var closure = new InstructionClosure();
                    closure.Parameters = p.Cdr.To<SchemePair>().Car.To<SchemePair>().AsEnumerable().Select(x => x.To<SchemeSymbol>().Value).ToArray();
                    closure.BodyIndex = instructions.Next;

                    foreach (var bodyExpression in p.Cdr.To<SchemePair>().Cdr.To<SchemePair>().AsEnumerable())
                        bodyExpression.To<SchemeDatum>().Compile(instructions);

                    instructions.Push(new InstructionReturn());

                    jump.Index = instructions.Next;

                    instructions.Push(closure);

                    break;

                case "call/cc":

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

                    p.Cdr.To<SchemePair>().Car.To<SchemeDatum>().Compile(instructions);
                    instructions.Push(new InstructionApply());

                    ccframe.Next = instructions.Next;

                    break;

                // (a b)
                default:
                    var frame = new InstructionFrame(0); // Continuation
                    instructions.Push(frame);

                    if (p.Cdr is not SchemeEmptyList)
                    {
                        foreach (var argument in p.Cdr.To<SchemePair>().AsEnumerable())
                        {
                            argument.To<SchemeDatum>().Compile(instructions);
                            instructions.Push(new InstructionArgument());
                        }
                    }

                    p.Car.To<SchemeSymbol>().Compile(instructions);
                    instructions.Push(new InstructionApply());

                    frame.Next = instructions.Next;

                    break;
            }
        }
        else // (expr b)
        {
            var frame = new InstructionFrame(0);
            instructions.Push(frame);

            foreach (var argument in p.Cdr.To<SchemePair>().AsEnumerable())
            {
                argument.To<SchemeDatum>().Compile(instructions);
                instructions.Push(new InstructionArgument());
            }

            p.Car.To<SchemeDatum>().Compile(instructions);
            instructions.Push(new InstructionApply());

            frame.Next = instructions.Next;
            /*
            
            (recur loop ([args (cdr x)]
                         [c (compile (car x) ’(apply))])
                (if (null? args)
                    (if (tail? next) c (list ’frame next c))
                    (loop (cdr args)
                          (compile (car args) (list ’argument c))
                    )
                )
            )

            */
        }
    }
}