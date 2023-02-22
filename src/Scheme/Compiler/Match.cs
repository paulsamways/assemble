using System.Diagnostics.CodeAnalysis;

namespace Scheme.Compiler;

public abstract class Match
{
    public static T MatchOrThrow<T>(SchemeObject input, Match<T> m)
        => m.Func(input).Value;

    public static bool TryMatch<T>(SchemeObject input, Match<T> m, [NotNullWhen(true)] out T? value)
        where T : class
    {
        var result = m.Func(input);
        if (result is Match<T>.Success success)
        {
            value = success.Value;
            return true;
        }
        value = default;
        return false;
    }

    public static Match<T> OneOf<T>(params Match<T>[] xs)
        where T : class
    {
        return new("one of " + string.Join(", ", xs.Select(x => x.Name)), (SchemeObject input) =>
        {
            foreach (var x in xs)
                if (x.Func(input) is Match<T>.Success result)
                    return result;

            return new Match<T>.Failure($"'{input} does not match one of {string.Join(", ", xs.Select(x => x.Name))}");
        });
    }

    public static Match<T> TypeOf<T>(string name)
        where T : SchemeObject
    {
        return new(name, (SchemeObject input) =>
            input.TryTo<T>(out var output, out _)
                ? output : null);
    }

    public readonly static Match<SchemeDatum> AnyDatum = TypeOf<SchemeDatum>("datum");
    public readonly static Match<SchemeBoolean> AnyBoolean = TypeOf<SchemeBoolean>("boolean");
    public readonly static Match<SchemeNumber> AnyNumber = TypeOf<SchemeNumber>("number");
    public readonly static Match<SchemeString> AnyString = TypeOf<SchemeString>("string");
    public readonly static Match<SchemeSymbol> AnySymbol = TypeOf<SchemeSymbol>("symbol");
    public readonly static Match<SchemePair> AnyPair = TypeOf<SchemePair>("pair");
    public readonly static Match<SchemeEmptyList> AnyEmptyList = TypeOf<SchemeEmptyList>("empty list");
    public readonly static Match<SchemeBytevector> AnyBytevector = TypeOf<SchemeBytevector>("bytevector");
    public readonly static Match<SchemeVector> AnyVector = TypeOf<SchemeVector>("vector");

    public static Match<SchemeSymbol> Symbol(string name)
    {
        return new("'" + name, (SchemeObject input) =>
            input.TryTo<SchemeSymbol>(out var output, out _) && output.Value.Equals(name)
                ? output : null);
    }

    public static Match<SchemeSymbol> Symbol(SchemeSymbol symbol)
    {
        return new("'" + symbol.Name, (SchemeObject input) =>
            input.TryTo<SchemeSymbol>(out var output, out _) && output.Equals(symbol)
                ? output : null);
    }

    public static Match<Tuple<SchemePair, SchemeObject, SchemeObject>> Pair()
    {
        return new("pair", (SchemeObject input) =>
            input.TryTo<SchemePair>(out var p, out _)
                ? new(p, p.Car, p.Cdr) : null);
    }

    public static Match<Tuple<SchemePair, SchemeObject[]>> List()
    {
        return new("list", (SchemeObject input) =>
            input.TryTo<SchemePair>(out var p, out _) && p.IsList
                ? new(p, p.ToEnumerable(true).ToArray()) : null);
    }

    public static Match<Tuple<SchemePair, T[]>> ListMany<T>(Match<T> a)
    {
        return new($"list of many {a.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var xs = p.ToEnumerable(true).ToArray();
                var ts = new T[xs.Length];
                for (var i = 0; i < xs.Length; i++)
                {
                    if (a.Func(xs[i]) is not Match<T>.Success success)
                        return null;

                    ts[i] = success.Value;
                }
                return new Tuple<SchemePair, T[]>(p, ts);
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, SchemeObject[], SchemeObject>> ImproperList()
    {
        return new("improper list", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable().ToArray();
                return new(p, elements[..^1].ToArray(), elements[^1]);
            }
            return null;
        });
    }

    public static Match<Tuple<SchemePair, T>> List<T>(Match<T> a)
    {
        return new($"list of {a.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length == 1)
                {
                    return a.Func(elements[0]) is Match<T>.Success pa
                        ? new Tuple<SchemePair, T>(p, pa.Value)
                        : null;
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB[]>> ListMany<TA, TB>(Match<TA> a, Match<TB> b)
    {
        return new($"list of {a.Name}, many {b.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length > 0)
                {
                    if (a.Func(elements[0]) is Match<TA>.Success pa)
                    {
                        var bs = new TB[elements.Length - 1];
                        for (var i = 1; i < elements.Length; i++)
                        {
                            if (b.Func(elements[i]) is not Match<TB>.Success success)
                                return null;
                            bs[i - 1] = success.Value;
                        }

                        return new Tuple<SchemePair, TA, TB[]>(p, pa.Value, bs);
                    }
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB>> List<TA, TB>(Match<TA> a, Match<TB> b)
    {
        return new($"list of {a.Name}, {b.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length == 2)
                {
                    return a.Func(elements[0]) is Match<TA>.Success pa && b.Func(elements[1]) is Match<TB>.Success pb
                        ? new Tuple<SchemePair, TA, TB>(p, pa.Value, pb.Value)
                        : null;
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB, TC[]>> ListMany<TA, TB, TC>(Match<TA> a, Match<TB> b, Match<TC> c)
    {
        return new($"list of {a.Name}, {b.Name}, many {c.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length > 1)
                {
                    if (a.Func(elements[0]) is Match<TA>.Success pa && b.Func(elements[1]) is Match<TB>.Success pb)
                    {
                        var cs = new TC[elements.Length - 2];
                        for (var i = 2; i < elements.Length; i++)
                        {
                            if (c.Func(elements[i]) is not Match<TC>.Success success)
                                return null;
                            cs[i - 2] = success.Value;
                        }

                        return new Tuple<SchemePair, TA, TB, TC[]>(p, pa.Value, pb.Value, cs);
                    }
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB, TC>> List<TA, TB, TC>(Match<TA> a, Match<TB> b, Match<TC> c)
    {
        return new($"list of {a.Name}, {b.Name}, {c.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length == 3)
                {
                    return a.Func(elements[0]) is Match<TA>.Success pa
                            && b.Func(elements[1]) is Match<TB>.Success pb
                            && c.Func(elements[2]) is Match<TC>.Success pc
                        ? new Tuple<SchemePair, TA, TB, TC>(p, pa.Value, pb.Value, pc.Value)
                        : null;
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB, TC, TD[]>> ListMany<TA, TB, TC, TD>(Match<TA> a, Match<TB> b, Match<TC> c, Match<TD> d)
    {
        return new($"list of {a.Name}, {b.Name}, {c.Name}, many {d.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length > 2)
                {
                    if (a.Func(elements[0]) is Match<TA>.Success pa
                        && b.Func(elements[1]) is Match<TB>.Success pb
                        && c.Func(elements[2]) is Match<TC>.Success pc)
                    {
                        var ds = new TD[elements.Length - 3];
                        for (var i = 3; i < elements.Length; i++)
                        {
                            if (d.Func(elements[i]) is not Match<TD>.Success success)
                                return null;
                            ds[i - 3] = success.Value;
                        }

                        return new Tuple<SchemePair, TA, TB, TC, TD[]>(p, pa.Value, pb.Value, pc.Value, ds);
                    }
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB, TC, TD>> List<TA, TB, TC, TD>(Match<TA> a, Match<TB> b, Match<TC> c, Match<TD> d)
    {
        return new($"list of {a.Name}, {b.Name}, {c.Name}, {d.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length == 4)
                {
                    return a.Func(elements[0]) is Match<TA>.Success pa
                            && b.Func(elements[1]) is Match<TB>.Success pb
                            && c.Func(elements[2]) is Match<TC>.Success pc
                            && d.Func(elements[3]) is Match<TD>.Success pd
                        ? new Tuple<SchemePair, TA, TB, TC, TD>(p, pa.Value, pb.Value, pc.Value, pd.Value)
                        : null;
                }
            }

            return null;
        });
    }

    public static Match<Tuple<SchemePair, TA, TB, TC, TD, TE[]>> ListMany<TA, TB, TC, TD, TE>(Match<TA> a, Match<TB> b, Match<TC> c, Match<TD> d, Match<TE> e)
    {
        return new($"list of {a.Name}, {b.Name}, {c.Name}, {d.Name}, many {e.Name}", (SchemeObject input) =>
        {
            if (input.TryTo<SchemePair>(out var p, out _))
            {
                var elements = p.ToEnumerable(true).ToArray();

                if (elements.Length > 3)
                {
                    if (a.Func(elements[0]) is Match<TA>.Success pa
                        && b.Func(elements[1]) is Match<TB>.Success pb
                        && c.Func(elements[2]) is Match<TC>.Success pc
                        && d.Func(elements[3]) is Match<TD>.Success pd)
                    {
                        var es = new TE[elements.Length - 4];
                        for (var i = 4; i < elements.Length; i++)
                        {
                            if (e.Func(elements[i]) is not Match<TE>.Success success)
                                return null;
                            es[i - 4] = success.Value;
                        }

                        return new Tuple<SchemePair, TA, TB, TC, TD, TE[]>(p, pa.Value, pb.Value, pc.Value, pd.Value, es);
                    }
                }
            }

            return null;
        });
    }
}

public sealed class Match<T> : Match
{
    public Match(string name, Func<SchemeObject, Result> m)
    {
        Name = name;
        Func = m;
    }

    public Match(string name, Func<SchemeObject, T?> m)
    {
        Name = name;
        Func = (o) =>
        {
            var x = m(o);
            return x is null
                ? new Failure($"{o} did not match a {Name}")
                : new Success(x);
        };
    }

    public string Name { get; private set; }

    public Func<SchemeObject, Result> Func { get; }

    public abstract class Result
    {
        public abstract T Value { get; }
    }
    public sealed class Success : Result
    {
        public Success(T value)
        {
            Value = value;
        }

        public override T Value { get; }
    }
    public sealed class Failure : Result
    {
        public Failure(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public override T Value => throw new Exception("match failure: " + Message);
    }
}