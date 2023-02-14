using System.Diagnostics;
using Scheme.Compiler;

namespace Scheme.Repl;

public static class Program
{
    private static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();
        var environment = Scheme.Compiler.Environment.Base();
        var vm = new VM(environment);

        ReadLine.AutoCompletionHandler = new AutoCompletionHandler(environment);
        ReadLine.HistoryEnabled = true;

        Console.Out.Flush();

        while (true)
        {
            stopwatch.Reset();

            var input = ReadLine.Read("λ > ");

            if (!string.IsNullOrEmpty(input))
            {
                if (input == ":q")
                    break;

                try
                {
                    stopwatch.Start();
                    var result = vm.Run((SchemeDatum)new Parser().Parse(input));
                    stopwatch.Stop();

                    if (result is not SchemeUndefined)
                    {
                        Console.WriteLine("... {0} ({1}ms)", result, stopwatch.ElapsedMilliseconds);

                        environment.Set(SchemeSymbol.FromString("$$"), result);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("--! " + e.Message);
                }

            }
        }
    }
}