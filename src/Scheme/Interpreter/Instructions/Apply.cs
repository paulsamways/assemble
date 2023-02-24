namespace Scheme.Interpreter.Instructions;

/// <summary>
/// The <c>Apply</c> instruction applies the closure in the accumulator to the list of values in the current
/// rib. Precisely, this instruction extends the closure’s environment with the closure’s
/// variable list and the current rib, sets the current environment to this new environment,
/// sets the current rib to the empty list, and sets the next expression to the closure’s body
/// </summary>
public class Apply : Instruction
{
    public override void Execute(VM vm)
    {
        if (vm.Accumulator is SchemeBuiltinProcedure p)
        {
            vm.Accumulator = p.Func(vm.Environment, vm.Ribs.ToArray());
            vm.Return();
        }
        else
        {
            var procedure = vm.Accumulator.To<SchemeProcedure>();
            var e = new Environment(procedure.Closure);

            if (vm.Ribs.Count != procedure.Parameters.Length)
                throw new Exception($"incorrect number of arguments, have {vm.Ribs.Count} but need {procedure.Parameters.Length}");

            foreach (var (k, v) in procedure.Parameters.Zip(vm.Ribs))
                e.Set(SchemeSymbol.FromString(k), v);

            vm.Apply(e, procedure.Body);
        }
    }
}