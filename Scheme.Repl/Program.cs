using System.Diagnostics;
using Scheme;
using Scheme.Compiler;

public static class Program
{
    private static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();
        var environment = Scheme.Compiler.Environment.Base();
        var vm = new VM(environment);

        ReadLine.AutoCompletionHandler = new AutoCompletionHandler(environment);
        ReadLine.HistoryEnabled = true;

        Console.WriteLine("Scheme R7RS REPL");
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
                    var result = vm.Run((SchemeDatum)Parser.Parse(input));
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