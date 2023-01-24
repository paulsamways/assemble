using Assemble;
using Assemble.Scheme;

internal class Program
{
    class AutoCompletionHandler : IAutoCompleteHandler
    {
        // characters to start completion from
        public char[] Separators { get; set; } = new char[] { '(', ' ' };

        // text - The current text entered in the console
        // index - The index of the terminal cursor within {text}
        public string[] GetSuggestions(string text, int index)
        {
            return new string[] { "cons", "car", "cdr" };
        }
    }

    private static void Main(string[] args)
    {
        var environment = Assemble.Scheme.Environment.Base();

        ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
        ReadLine.HistoryEnabled = true;

        Console.WriteLine("Welcome to Assemble!");
        Console.Out.Flush();

        while (true)
        {
            var input = ReadLine.Read("scheme> ");
            if (!string.IsNullOrEmpty(input))
            {
                if (input == ":q")
                    break;

                try
                {
                    var result = Parser.Parse(input).Evaluate(environment);

                    if (result is not SchemeUndefined)
                        Console.WriteLine("------> " + result.Write());
                }
                catch (Exception e)
                {
                    Console.WriteLine("------! " + e.Message);
                }

            }
        }
    }
}