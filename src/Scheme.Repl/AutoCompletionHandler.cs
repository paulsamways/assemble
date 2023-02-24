namespace Scheme.Repl;

internal sealed class AutoCompletionHandler : IAutoCompleteHandler
{
    private readonly string[] _globalBindings;

    public AutoCompletionHandler(Interpreter.Environment e)
    {
        var names = new SortedSet<string>
            {
                "quote",
                "if",
                "lambda",
                "set!",
                "call/cc",
                "call-with-current-continuation"
            };
        foreach (var name in e.GetNames())
            names.Add(name);

        _globalBindings = names.ToArray();
    }

    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { '(', ' ' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        return _globalBindings;
    }
}