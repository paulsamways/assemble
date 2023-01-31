using Assemble.Scheme.Compiler.Instructions;

namespace Assemble.Scheme.Compiler;

public class InstructionList
{
    private readonly List<Instruction> _instructions;

    public InstructionList()
    {
        _instructions = new List<Instruction>();
    }

    public int Next => _instructions.Count;

    public int Push(Instruction instruction)
    {
        var index = _instructions.Count;
        _instructions.Add(instruction);
        return index;
    }

    public Instruction[] ToArray() => _instructions.ToArray();

    public override string ToString()
    {
        return string.Join(System.Environment.NewLine, _instructions.Select((inst, i) => $"{i}: {inst}"));
    }
}