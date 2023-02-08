namespace Scheme.Compiler.Instructions;

/// <summary>
/// The <c>Test</c> instruction tests the accumulator and if the accumulator is non null (that is,
/// the test returned true), sets the next expression to <c>Then</c>. Otherwise test sets the next
/// expression to <c>Else</c>.
/// </summary>
public class Test : Instruction
{
    public Test(Instruction then, Instruction @else)
    {
        Then = then;
        Else = @else;
    }

    public Instruction Then { get; set; }

    public Instruction Else { get; set; }

    public override void Execute(VM vm)
    {
        vm.Next = SchemeBoolean.FromObject(vm.Accumulator).Value
            ? Then
            : Else;
    }
}