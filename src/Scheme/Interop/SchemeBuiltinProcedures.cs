namespace Scheme.Interop;

using Scheme.Compiler;

public static class SchemeBuiltinProcedures
{
    [SchemeBuiltinProcedure("not")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Not
        = SchemeBuiltinProcedure.Unary((x) => !SchemeBoolean.FromObject(x).Value);

    [SchemeBuiltinProcedure("boolean?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsBoolean
        = SchemeBuiltinProcedure.Is<SchemeBoolean>();

    [SchemeBuiltinProcedure("boolean=?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsBooleanEq
        = SchemeBuiltinProcedure.IsEq<SchemeBoolean>();

    [SchemeBuiltinProcedure("char?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsCharacter
        = SchemeBuiltinProcedure.Is<SchemeCharacter>();

    [SchemeBuiltinProcedure("char=?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsCharacterEq
        = SchemeBuiltinProcedure.IsEq<SchemeCharacter>();

    [SchemeBuiltinProcedure("symbol?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsSymbol
        = SchemeBuiltinProcedure.Is<SchemeSymbol>();

    [SchemeBuiltinProcedure("symbol=?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsSymbolEq
        = SchemeBuiltinProcedure.IsEq<SchemeSymbol>();

    [SchemeBuiltinProcedure("string?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsString
        = SchemeBuiltinProcedure.Is<SchemeString>();

    [SchemeBuiltinProcedure("string=?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsStringEq
        = SchemeBuiltinProcedure.IsEq<SchemeString>();

    [SchemeBuiltinProcedure("number?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsNumber
    = SchemeBuiltinProcedure.Is<SchemeNumber>();

    [SchemeBuiltinProcedure("number=?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsNumberEq
        = SchemeBuiltinProcedure.IsEq<SchemeNumber>();

    [SchemeBuiltinProcedure("even?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Even
        = SchemeBuiltinProcedure.Unary<SchemeNumber, decimal, SchemeBoolean, bool>(x => x % 2 == 0);

    [SchemeBuiltinProcedure("odd?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Odd
        = SchemeBuiltinProcedure.Unary<SchemeNumber, decimal, SchemeBoolean, bool>(x => x % 2 != 0);

    [SchemeBuiltinProcedure("eqv?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsEqv
        = SchemeBuiltinProcedure.Binary((x, y) => x.Same(y));

    [SchemeBuiltinProcedure("eq?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsEq
        = IsEqv;

    [SchemeBuiltinProcedure("equal?")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> IsEqual
        = SchemeBuiltinProcedure.Binary((x, y) => x.Equals(y));

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

    [SchemeBuiltinProcedure("<")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> LessThan
         = SchemeBuiltinProcedure.Binary<SchemeNumber, decimal, SchemeNumber, decimal, SchemeBoolean, bool>((a, b) => a < b);

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

    [SchemeBuiltinProcedure("read")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Read
        = SchemeBuiltinProcedure.Unary<SchemeString, SchemeObject>((x) => new Parser().Parse(x.Value));

    [SchemeBuiltinProcedure("read-syntax")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> ReadSyntax
        = SchemeBuiltinProcedure.Unary<SchemeString, SchemeObject>((x) => new Parser(true).Parse(x.Value));

    [SchemeBuiltinProcedure("eval")]
    public static readonly Func<Environment, SchemeObject[], SchemeObject> Eval
        = SchemeBuiltinProcedure.Unary<SchemeDatum, SchemeObject>((x) => new VM().Run(x));

    // [SchemeBuiltinProcedure("apply")]
    // public static SchemeObject Apply(Environment e, SchemeObject[] xs)
    // {
    //     return xs[0].To<SchemeCallable>().Call(e, SchemePair.FromEnumerable(xs[1..]));
    // }
}