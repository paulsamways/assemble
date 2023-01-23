using Assemble.Scheme;

internal class Program
{
    private static void Main(string[] args)
    {
        var environment = Assemble.Scheme.Environment.Default();

        Console.WriteLine("Welcome to Assemble!");
        Console.Out.Flush();

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                if (input == ":q")
                    break;

                try
                {
                    var result = Parser.Parse(input).Evaluate(environment);

                    if (result is not SchemeUndefined)
                        Console.WriteLine("= " + result.Write());
                }
                catch (Exception e)
                {
                    Console.WriteLine("! " + e.Message);
                }

            }
        }
    }
}