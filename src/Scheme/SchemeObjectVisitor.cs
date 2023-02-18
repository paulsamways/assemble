namespace Scheme;

public class SchemeObjectVisitor
{
    private readonly static Func<SchemeObject, SchemeObject> _identity = (x) => x;

    public SchemeObjectVisitor(bool changeStructure = true)
    {
        if (changeStructure)
        {
            OnSchemePair = (x, car, cdr) => new SchemePair(car, cdr);
            OnSchemeSyntaxObject = (x, datum) => new SchemeSyntaxObject(datum, x.Source, x.Scope.ToArray());
            OnSchemeVector = (x, xs) => new SchemeVector(xs.ToArray());
        }
        else
        {
            OnSchemePair = (x, car, cdr) => x;
            OnSchemeSyntaxObject = (x, datum) => x;
            OnSchemeVector = (x, xs) => x;
        }
    }

    public Func<SchemeBoolean, SchemeObject> OnSchemeBoolean { get; set; } = _identity;

    public Func<SchemeBuiltinProcedure, SchemeObject> OnSchemeBuiltinProcedure { get; set; } = _identity;

    public Func<SchemeBytevector, SchemeObject> OnSchemeBytevector { get; set; } = _identity;

    public Func<SchemeCharacter, SchemeObject> OnSchemeCharacter { get; set; } = _identity;

    public Func<SchemeEmptyList, SchemeObject> OnSchemeEmptyList { get; set; } = _identity;

    public Func<SchemeNumber, SchemeObject> OnSchemeNumber { get; set; } = _identity;

    public Func<SchemePair, SchemeObject, SchemeObject, SchemeObject> OnSchemePair { get; set; }

    public Func<SchemeProcedure, SchemeObject> OnSchemeProcedure { get; set; } = _identity;

    public Func<SchemeString, SchemeObject> OnSchemeString { get; set; } = _identity;

    public Func<SchemeSymbol, SchemeObject> OnSchemeSymbol { get; set; } = _identity;

    public Func<SchemeSyntaxObject, SchemeDatum, SchemeObject> OnSchemeSyntaxObject { get; set; }

    public Func<SchemeUndefined, SchemeObject> OnSchemeUndefined { get; set; } = _identity;

    public Func<SchemeVector, SchemeObject[], SchemeObject> OnSchemeVector { get; set; }
}