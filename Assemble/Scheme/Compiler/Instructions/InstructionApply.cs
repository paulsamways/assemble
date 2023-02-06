namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionApply : Instruction
{
    public override void Execute(Interpreter interpreter)
    {
        if (interpreter.Accumulator is SchemeBuiltinProcedure p)
        {
            interpreter.Accumulator = p.Func(interpreter.Environment, interpreter.Ribs.ToArray());
            interpreter.Return();
        }
        else
        {
            var procedure = interpreter.Accumulator.To<SchemeProcedure>();
            var e = new Environment(procedure.Closure);

            if (interpreter.Ribs.Count != procedure.Parameters.Length)
                throw new Exception($"incorrect number of arguments, have {interpreter.Ribs.Count} but need {procedure.Parameters.Length}");

            foreach (var (k, v) in procedure.Parameters.Zip(interpreter.Ribs))
                e.Set(SchemeSymbol.FromString(k), v);

            interpreter.Apply(e, procedure.Body);
        }
    }

    public override string ToString()
    {
        return $"APP";
    }
}