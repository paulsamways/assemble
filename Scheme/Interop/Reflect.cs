using System.Linq.Expressions;
using System.Reflection;

namespace Scheme.Interop;

public static class Reflect
{
    public static void LoadBuiltinProceduresFromAssembly<T>(Compiler.Environment e)
    {
        foreach (var t in typeof(T).Assembly.ExportedTypes)
        {
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

                            var expr = Expression.Lambda<Func<Compiler.Environment, SchemeObject[], SchemeObject>>(
                                Expression.Call(null, method, pEnvironment, pArguments),
                                false,
                                pEnvironment,
                                pArguments
                            );

                            var f = expr.Compile();

                            e.Set(SchemeSymbol.FromString(mAttr.Name), new SchemeBuiltinProcedure(f));
                        }
                    }
                }

                foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var fAttr = field.GetCustomAttribute<SchemeBuiltinProcedureAttribute>();

                    if (fAttr is not null)
                    {
                        SchemeObject p;

                        if (field.FieldType == typeof(Func<Compiler.Environment, SchemeObject[], SchemeObject>))
                        {
                            p = new SchemeBuiltinProcedure(
                                (Func<Compiler.Environment, SchemeObject[], SchemeObject>)field.GetValue(null)!
                            );

                        }
                        else
                        {
                            throw new Exception("SchemeBuiltinProcedure can only be used on func or string");
                        }

                        e.Set(SchemeSymbol.FromString(fAttr.Name), p);
                    }
                }
            }
        }
    }
}