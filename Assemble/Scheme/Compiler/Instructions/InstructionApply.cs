namespace Assemble.Scheme.Compiler.Instructions;

public class InstructionApply : Instruction
{
    public InstructionApply()
    {
    }

    public override SchemeObject Execute(SchemeObject accumulator, Interpreter interpreter)
    {
        if (accumulator is SchemeBuiltinProcedure p)
        {
            var result = p.Func(interpreter.Environment, interpreter.Ribs.ToArray());
            interpreter.Return();
            return result;
        }

        var procedure = accumulator.To<SchemeProcedure>();
        var e = new Environment(procedure.Closure);

        if (interpreter.Ribs.Count != procedure.Parameters.Length)
            throw new Exception($"incorrect number of arguments, have {interpreter.Ribs.Count} but need {procedure.Parameters.Length}");

        foreach (var (k, v) in procedure.Parameters.Zip(interpreter.Ribs))
            e.Set(SchemeSymbol.FromString(k), v);

        interpreter.Apply(e, procedure.BodyIndex);

        return accumulator;
    }

    public override string ToString()
    {
        return $"APP";
    }
}