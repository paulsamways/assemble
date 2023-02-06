namespace Assemble.Scheme;

public abstract class SchemeObject : IEquatable<SchemeObject>
{
    public virtual T To<T>() where T : SchemeObject
    {
        if (this is T t)
            return t;

        throw new Exception($"Type error: have {GetType()} but wanted {typeof(T)}");
    }

    public static implicit operator SchemeObject(bool b) => SchemeBoolean.FromBoolean(b);

    public static implicit operator SchemeObject(string s) => new SchemeString(s);

    public static implicit operator SchemeObject(char c) => new SchemeCharacter(c);

    public static implicit operator SchemeObject(decimal d) => new SchemeNumber(d);

    public static implicit operator SchemeObject(byte[] bs) => new SchemeBytevector(bs);

    public static implicit operator SchemeObject(Func<Environment, SchemeObject, SchemeObject> f)
        => new SchemeBuiltinProcedure((e, xs) => f(e, xs[0]));


    public abstract string Name { get; }

    public override string ToString()
    {
        return Write();
    }

    public virtual string Write()
    {
        return $"<{Name}>";
    }

    public abstract bool Equals(SchemeObject? other);

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || Equals(obj as SchemeObject);
    }

    public virtual bool Same(SchemeObject other) => ReferenceEquals(this, other);
}