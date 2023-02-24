namespace Scheme;

public class SchemeObjectVisitor
{
    public SchemeObjectVisitor(bool changeStructure = true)
    {
        if (changeStructure)
        {
            OnSchemePair = (x, car, cdr) => Default(new SchemePair(car, cdr));
            OnSchemeSyntaxObject = (x, datum) => Default(new SchemeSyntaxObject(datum, x.Source, x.Scope.ToArray()));
            OnSchemeVector = (x, xs) => Default(new SchemeVector(xs.ToArray()));
        }
        else
        {
            OnSchemePair = (x, car, cdr) => Default(x);
            OnSchemeSyntaxObject = (x, datum) => Default(x);
            OnSchemeVector = (x, xs) => Default(x);
        }

        OnSchemeBoolean = Default;
        OnSchemeBoolean = Default;
        OnSchemeBuiltinProcedure = Default;
        OnSchemeBytevector = Default;
        OnSchemeCharacter = Default;
        OnSchemeEmptyList = Default;
        OnSchemeNumber = Default;
        OnSchemeProcedure = Default;
        OnSchemeString = Default;
        OnSchemeSymbol = Default;
        OnSchemeUndefined = Default;

    }

    public Func<SchemeObject, SchemeObject> Otherwise { get; set; } = (x) => x;

    public Func<SchemeBoolean, SchemeObject> OnSchemeBoolean { get; set; }

    public Func<SchemeBuiltinProcedure, SchemeObject> OnSchemeBuiltinProcedure { get; set; }

    public Func<SchemeBytevector, SchemeObject> OnSchemeBytevector { get; set; }

    public Func<SchemeCharacter, SchemeObject> OnSchemeCharacter { get; set; }

    public Func<SchemeEmptyList, SchemeObject> OnSchemeEmptyList { get; set; }

    public Func<SchemeNumber, SchemeObject> OnSchemeNumber { get; set; }

    public Func<SchemePair, SchemeObject, SchemeObject, SchemeObject> OnSchemePair { get; set; }

    public Func<SchemeProcedure, SchemeObject> OnSchemeProcedure { get; set; }

    public Func<SchemeString, SchemeObject> OnSchemeString { get; set; }

    public Func<SchemeSymbol, SchemeObject> OnSchemeSymbol { get; set; }

    public Func<SchemeSyntaxObject, SchemeDatum, SchemeObject> OnSchemeSyntaxObject { get; set; }

    public Func<SchemeUndefined, SchemeObject> OnSchemeUndefined { get; set; }

    public Func<SchemeVector, SchemeObject[], SchemeObject> OnSchemeVector { get; set; }

    private SchemeObject Default(SchemeObject o)
        => Otherwise(o);
}