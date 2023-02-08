namespace Scheme.Compiler.Instructions;

public class Closure : Instruction
{
    public Closure(string[] parameters, Instruction body, Instruction next)
    {
        Parameters = parameters;
        Body = body;
        Next = next;
    }

    public string[] Parameters { get; set; }

    public Instruction Next { get; set; }

    public Instruction Body { get; set; }

    public SchemeDatum? Source { get; set; }

    public override void Execute(VM vm)
    {
        vm.Accumulator = new SchemeProcedure(vm.Environment, Parameters, Body);
        vm.Next = Next;
    }

    public override string ToString()
    {
        return $"CLSR {Source}";
    }
}