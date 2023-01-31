using Assemble.Scheme.Compiler;
using Assemble.Scheme.Compiler.Instructions;

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

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemePair p && p.Car.Equals(Car) && p.Cdr.Equals(Cdr);

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

    public override void Compile(InstructionList instructions)
    {
        if (Car is SchemeSymbol s)
        {
            switch (s.Value)
            {
                case "quote":
                    instructions.Push(new InstructionConstant(Cdr.To<SchemePair>().Car.To<SchemeDatum>()));
                    break;
                case "set!":
                    Cdr.To<SchemePair>().Cdr.To<SchemePair>().Car.To<SchemeDatum>().Compile(instructions);

                    instructions.Push(new InstructionAssign(Cdr.To<SchemePair>().Car.To<SchemeSymbol>().Value));
                    break;
                case "if":

                    var jif = new InstructionConditionalJump(int.MaxValue, false);
                    var j = new InstructionJump(int.MaxValue);

                    var cdr = Cdr.To<SchemePair>();
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
                    closure.Parameters = Cdr.To<SchemePair>().Car.To<SchemePair>().AsEnumerable().Select(x => x.To<SchemeSymbol>().Value).ToArray();
                    closure.BodyIndex = instructions.Next;

                    foreach (var bodyExpression in Cdr.To<SchemePair>().Cdr.To<SchemePair>().AsEnumerable())
                        bodyExpression.To<SchemeDatum>().Compile(instructions);

                    instructions.Push(new InstructionReturn());

                    jump.Index = instructions.Next;

                    instructions.Push(closure);

                    break;
                // (a b)
                default:
                    var frame = new InstructionFrame(0);
                    instructions.Push(frame);

                    if (Cdr is not SchemeEmptyList)
                    {
                        foreach (var argument in Cdr.To<SchemePair>().AsEnumerable())
                        {
                            argument.To<SchemeDatum>().Compile(instructions);
                            instructions.Push(new InstructionArgument());
                        }
                    }

                    Car.To<SchemeSymbol>().Compile(instructions);
                    instructions.Push(new InstructionApply());

                    frame.Next = instructions.Next;

                    break;
            }
        }
        else // (expr b)
        {
            var frame = new InstructionFrame(0);
            instructions.Push(frame);

            foreach (var argument in Cdr.To<SchemePair>().AsEnumerable())
            {
                argument.To<SchemeDatum>().Compile(instructions);
                instructions.Push(new InstructionArgument());
            }

            Car.To<SchemeDatum>().Compile(instructions);
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
