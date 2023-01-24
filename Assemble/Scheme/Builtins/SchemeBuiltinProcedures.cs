namespace Assemble.Scheme.Builtins;

public static class SchemeBuiltinProcedures
{
    [SchemeBuiltinProcedure("not")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Not
        = SchemeBuiltinProcedure.Unary<SchemeBoolean, bool>((x) => !x);

    [SchemeBuiltinProcedure("+")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Add
        = SchemeBuiltinProcedure.NAry<SchemeNumber, decimal>((a, b) => a + b);

    [SchemeBuiltinProcedure("-")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Subtract
        = SchemeBuiltinProcedure.NAry<SchemeNumber, decimal>((a, b) => a - b);

    [SchemeBuiltinProcedure("*")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Multiply
        = SchemeBuiltinProcedure.NAry<SchemeNumber, decimal>((a, b) => a * b);

    [SchemeBuiltinProcedure("/")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Divide
        = SchemeBuiltinProcedure.NAry<SchemeNumber, decimal>((a, b) => a / b);

    [SchemeBuiltinProcedure("inc")]
    public const string Inc = "(lambda (a) (+ a 1))";

    [SchemeBuiltinProcedure("null?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsNull
        = SchemeBuiltinProcedure.Unary(x => x is SchemeEmptyList);

    [SchemeBuiltinProcedure("list?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsList
        = SchemeBuiltinProcedure.Unary(x => x is SchemePair p && p.IsList);

    [SchemeBuiltinProcedure("pair?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsPair
        = SchemeBuiltinProcedure.Unary(x => x is SchemePair);

    [SchemeBuiltinProcedure("cons")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Cons
        = SchemeBuiltinProcedure.Binary((x, y) => new SchemePair(x, y));

    [SchemeBuiltinProcedure("car")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Car
        = SchemeBuiltinProcedure.Unary<SchemePair, SchemeObject>((x) => x.Car);

    [SchemeBuiltinProcedure("cdr")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Cdr
        = SchemeBuiltinProcedure.Unary<SchemePair, SchemeObject>((x) => x.Cdr);

    [SchemeBuiltinProcedure("set-car!")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> SetCar
        = SchemeBuiltinProcedure.Binary<SchemePair, SchemeObject, SchemeUndefined>((x, car) =>
        {
            x.Car = car;
            return SchemeUndefined.Value;
        });

    [SchemeBuiltinProcedure("set-cdr!")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> SetCdr
        = SchemeBuiltinProcedure.Binary<SchemePair, SchemeObject, SchemeUndefined>((x, cdr) =>
        {
            x.Cdr = cdr;
            return SchemeUndefined.Value;
        });

    [SchemeBuiltinProcedure("apply")]
    public static SchemeObject Apply(Environment e, SchemeObject[] xs)
    {
        return xs[0].To<SchemeProcedure>().Call(e, SchemePair.FromEnumerable(xs[1..]));
    }
}