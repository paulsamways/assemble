namespace Scheme;

using Scheme.Compiler;

public sealed class SchemeBuiltinProcedure : SchemeObject
{
    public SchemeBuiltinProcedure(Func<Environment, SchemeObject[], SchemeObject> func)
    {
        Func = func;
    }

    public Func<Environment, SchemeObject[], SchemeObject> Func { get; }

    public override string Name => "procedure";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeBuiltinProcedure p && Func == p.Func;

    public override int GetHashCode() => Func.GetHashCode();

    public static Func<Environment, SchemeObject[], SchemeObject> Is<T>()
        where T : SchemeObject
        => Unary((x) => x is T);

    public static Func<Environment, SchemeObject[], SchemeObject> IsEq<T>()
        where T : SchemeObject
        => NAry<T, SchemeBoolean>((xs) => xs.Length == 0
            ? SchemeBoolean.True
            : SchemeBoolean.FromBoolean(xs.All(x => x.Equals(xs[0]))));

    public static Func<Environment, SchemeObject[], SchemeObject> NAry<T, U>(Func<T[], U> func)
        where T : SchemeObject
        where U : SchemeObject
        => (_, xs) => func(xs.Select(x => x.To<T>()).ToArray());

    public static Func<Environment, SchemeObject[], SchemeObject> NAry<T, U>(Func<U, U, U> func)
        where T : SchemeObject, Wraps<T, U>
    {
        return (_, xs) => T.Wrap(xs.Select(x => x.To<T>().Unwrap()).Aggregate(func));
    }

    public static Func<Environment, SchemeObject[], SchemeObject> Unary(Func<SchemeObject, SchemeObject> func)
    {
        return (_, xs) =>
        {
            if (xs.Length != 1)
                throw new Exception("wrong number of args");

            return func(xs[0]);
        };
    }

    public static Func<Environment, SchemeObject[], SchemeObject> Unary<T, R>(Func<T, R> func)
        where T : SchemeObject
        where R : SchemeObject
        => Unary((x) => func(x.To<T>()));

    public static Func<Environment, SchemeObject[], SchemeObject> Unary<TSchemeObject, T>(Func<T, T> func)
        where TSchemeObject : SchemeObject, Wraps<TSchemeObject, T>
        => Unary<TSchemeObject, T, TSchemeObject, T>(func);

    public static Func<Environment, SchemeObject[], SchemeObject> Unary<TSchemeObject, T, TResultSchemeObject, TResult>(Func<T, TResult> func)
        where TSchemeObject : SchemeObject, Wraps<TSchemeObject, T>
        where TResultSchemeObject : SchemeObject, Wraps<TResultSchemeObject, TResult>
        => Unary((x) => TResultSchemeObject.Wrap(func(x.To<TSchemeObject>().Unwrap())));

    public static Func<Environment, SchemeObject[], SchemeObject> Binary(Func<SchemeObject, SchemeObject, SchemeObject> func)
    {
        return (_, xs) =>
        {
            if (xs.Length != 2)
                throw new Exception("wrong number of args");

            return func(xs[0], xs[1]);
        };
    }

    public static Func<Environment, SchemeObject[], SchemeObject> Binary<T, U, R>(Func<T, U, R> func)
        where T : SchemeObject
        where U : SchemeObject
        where R : SchemeObject
        => Binary((x, y) => func(x.To<T>(), y.To<U>()));

    public static Func<Environment, SchemeObject[], SchemeObject> Binary<T1SchemeObject, T1, T2SchemeObject, T2, TResultSchemeObject, TResult>(Func<T1, T2, TResult> func)
        where T1SchemeObject : SchemeObject, Wraps<T1SchemeObject, T1>
        where T2SchemeObject : SchemeObject, Wraps<T2SchemeObject, T2>
        where TResultSchemeObject : SchemeObject, Wraps<TResultSchemeObject, TResult>
        => Binary((x, y) => TResultSchemeObject.Wrap(
            func(
                x.To<T1SchemeObject>().Unwrap(),
                y.To<T2SchemeObject>().Unwrap()
            )
        ));
}

public interface Wraps<T, U>
    where T : Wraps<T, U>
{
    U Unwrap();

    static abstract T Wrap(U value);
}