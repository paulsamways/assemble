using System.Linq.Expressions;
using System.Reflection;

namespace Assemble.Scheme;

public class Environment
{
    private readonly Environment? _parent;

    private readonly Dictionary<string, SchemeObject> _objects;

    public Environment(Environment? parent = null)
    {
        _objects = new();
        _parent = parent;
    }

    public SchemeObject? Get(SchemeSymbol symbol)
    {
        if (_objects.TryGetValue(symbol.Value, out var o))
            return o;

        if (_parent is not null)
            return _parent.Get(symbol);

        return null;
    }
    public void Set(SchemeSymbol symbol, SchemeObject o)
    {
        _objects[symbol.Value] = o;
    }

    public void Load<T>()
    {
        foreach (var t in typeof(T).Assembly.ExportedTypes)
        {
            var attr = t.GetCustomAttribute<SchemeBuiltinAttribute>();
            if (attr is not null)
            {
                if (t.IsAssignableTo(typeof(SchemeBuiltin)))
                {
                    var instance = (SchemeBuiltin)Activator.CreateInstance(t)!;
                    Set(SchemeSymbol.FromString(instance.Name), instance);
                }
            }

            if (t.IsAbstract && t.IsSealed)
            {
                foreach (var method in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var mAttr = method.GetCustomAttribute<SchemeBuiltinProcedureAttribute>();
                    if (mAttr is not null)
                    {
                        if (method.ReturnType == typeof(SchemeObject) &&
                            method.GetParameters()[0].ParameterType == typeof(Environment) &&
                            method.GetParameters()[1].ParameterType == typeof(SchemeObject[]))
                        {
                            var pEnvironment = Expression.Parameter(typeof(Environment), "e");
                            var pArguments = Expression.Parameter(typeof(SchemeObject[]), "xs");

                            var expr = Expression.Lambda<Func<Environment, SchemeObject[], SchemeObject>>(
                                Expression.Call(null, method, pEnvironment, pArguments),
                                false,
                                pEnvironment,
                                pArguments
                            );

                            var f = expr.Compile();

                            Set(SchemeSymbol.FromString(mAttr.Name), new SchemeBuiltinProcedure(f));
                        }
                    }
                }

                foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var fAttr = field.GetCustomAttribute<SchemeBuiltinProcedureAttribute>();

                    if (fAttr is not null)
                    {
                        SchemeCallable callable;

                        if (field.FieldType == typeof(Func<Environment, SchemeObject[], SchemeObject>))
                        {
                            callable = new SchemeBuiltinProcedure(
                                (Func<Environment, SchemeObject[], SchemeObject>)field.GetValue(null)!
                            );

                        }
                        else if (field.FieldType == typeof(string))
                        {
                            callable = Parser.Parse((string)field.GetValue(null)!).Evaluate(this).To<SchemeProcedure>();
                        }
                        else
                        {
                            throw new Exception("SchemeBuiltinProcedure can only be used on func or string");
                        }

                        Set(SchemeSymbol.FromString(fAttr.Name), callable);
                    }
                }
            }
        }
    }

    public static Environment Base()
    {
        var e = new Environment();
        e.Load<Environment>();
        return e;
    }
}