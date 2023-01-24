namespace Assemble.Scheme;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
public sealed class SchemeBuiltinProcedureAttribute : Attribute
{
    public SchemeBuiltinProcedureAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
}