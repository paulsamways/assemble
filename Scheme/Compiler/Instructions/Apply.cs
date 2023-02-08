namespace Scheme.Compiler.Instructions;

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