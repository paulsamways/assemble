using Assemble.Interpreter;

internal class Program
{
    private static void Main(string[] args)
    {
        var interpreter = new Interpreter();

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
                    var result = interpreter.Evaluate(input);

                    Console.WriteLine("= " + result.Print());
                }
                catch (Exception e)
                {
                    Console.WriteLine("! " + e.Message);
                }

            }
        }
    }
}