using System.Diagnostics;
using Assemble;
using Assemble.Scheme;
using Assemble.Scheme.Compiler;

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


    private static int Fib(int n)
    {
        if (n < 2) return n;
        return Fib(n - 1) + Fib(n - 2);
    }

    // Fib(38)  = 39088169 (184ms)
    // (fib 38) = 39088169 (136858ms)

    private static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();
        // stopwatch.Start();
        // var result2 = Fib(40);
        // stopwatch.Stop();
        // System.Console.WriteLine("{0} ({1}ms)", result2, stopwatch.ElapsedMilliseconds);
        //return;

        var environment = Assemble.Scheme.Environment.Base();
        var interpreter = new Interpreter(environment);

        ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
        ReadLine.HistoryEnabled = true;

        Console.WriteLine("Welcome to Assemble!");
        Console.Out.Flush();

        while (true)
        {
            stopwatch.Reset();

            var input = ReadLine.Read("scheme> ");
            if (!string.IsNullOrEmpty(input))
            {
                if (input == ":q")
                    break;

                try
                {
                    stopwatch.Start();
                    var result = interpreter.Run((SchemeDatum)Parser.Parse(input));
                    stopwatch.Stop();

                    if (result is not SchemeUndefined)
                        Console.WriteLine("------> {0} ({1}ms)", result.Write(), stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine("------! " + e);
                }

            }
        }
    }
}