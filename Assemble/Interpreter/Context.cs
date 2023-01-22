using Assemble.Interpreter.AST;

namespace Assemble.Interpreter;

public class Context
{
    private readonly Dictionary<string, Expression> _variables;

    private readonly Context? _parent;

    public Context()
    {
        _variables = new Dictionary<string, Expression>
        {
            ["+"] = NumberBinaryOp((a, b) => a + b),
            ["-"] = NumberBinaryOp((a, b) => a - b),
            ["*"] = NumberBinaryOp((a, b) => a * b),
            ["/"] = NumberBinaryOp((a, b) => a / b),

            ["equals"] = BooleanBinaryOp<IEquatable<Expression>>((a, b) => a.Equals((Expression)b)),
            ["not"] = new PrimitiveFunction((xs) => new AST.Boolean(!xs.Single().To<AST.Boolean>().Value)),

            ["set"] = new SetPrimitiveFunction(),
            ["if"] = new IfFunction(),
            ["lambda"] = new LambdaFunction()
        };
    }

    public Context(Context parent)
        : this()
    {
        _parent = parent;
    }

    public Expression Get(string name)
    {
        if (_variables.TryGetValue(name, out var v))
            return v;

        if (_parent is not null)
            return _parent.Get(name);

        throw new Exception("not found");
    }

    public void Set(string name, Expression value)
    {
        _variables[name] = value;
    }

    public void Set(IEnumerable<(string, Expression)> values)
    {
        foreach (var (k, v) in values)
            Set(k, v);
    }

    public bool Has(string name)
    {
        return _variables.ContainsKey(name) || (_parent?.Has(name) == true);
    }

    private static PrimitiveFunction NumberBinaryOp(Func<decimal, decimal, decimal> f)
    {
        return new PrimitiveFunction((xs) => new Number(
            xs
                .Select(x => x.To<Number>().Value)
                .Aggregate(f)
        ));
    }

    private static PrimitiveFunction BooleanBinaryOp<T>(Func<T, T, bool> f)
    {
        return new PrimitiveFunction((xs) =>
        {
            if (xs.Length != 2)
                throw new Exception("Wrong number of arguments");

            if (xs[0] is not T a)
                throw new Exception("Argument 0 is not the right type");

            if (xs[1] is not T b)
                throw new Exception("Argument 1 is not the right type");

            return new AST.Boolean(f(a, b));
        });
    }
}