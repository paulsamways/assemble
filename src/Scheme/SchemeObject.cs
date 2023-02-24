namespace Scheme;

using System.Diagnostics.CodeAnalysis;
using Scheme.Interpreter;

public abstract class SchemeObject : IEquatable<SchemeObject>
{
    public virtual bool TryTo<T>([NotNullWhen(true)] out T? value, out SourceInfo? source)
        where T : SchemeObject
    {
        if (this is T t)
        {
            value = t;
            source = null;
            return true;
        }

        if (this is SchemeSyntaxObject o && o.Datum is T datum)
        {
            value = datum;
            source = o.Source;
            return true;
        }

        value = null;
        source = null;
        return false;
    }

    public virtual T To<T>() where T : SchemeObject
        => To<T>(out SourceInfo? _);

    public virtual T To<T>(out SourceInfo? s) where T : SchemeObject
    {
        if (TryTo(out T? t, out s))
            return t;

        throw new Exception($"Type error: have {GetType()} but wanted {typeof(T)}");
    }

    public abstract string Name { get; }

    public abstract SchemeObject Visit(SchemeObjectVisitor v);

    public override string ToString() => $"<{Name}>";

    public abstract override int GetHashCode();

    public abstract bool Equals(SchemeObject? other);

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || Equals(obj as SchemeObject);
    }

    public virtual bool Same(SchemeObject other) => ReferenceEquals(this, other);

    public static implicit operator SchemeObject(bool b) => SchemeBoolean.FromBoolean(b);

    public static implicit operator SchemeObject(string s) => new SchemeString(s);

    public static implicit operator SchemeObject(char c) => new SchemeCharacter(c);

    public static implicit operator SchemeObject(decimal d) => new SchemeNumber(d);

    public static implicit operator SchemeObject(byte[] bs) => new SchemeBytevector(bs);

    public static implicit operator SchemeObject(Func<Environment, SchemeObject, SchemeObject> f)
        => new SchemeBuiltinProcedure((e, xs) => f(e, xs[0]));
}