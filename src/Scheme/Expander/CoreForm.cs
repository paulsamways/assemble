namespace Scheme.Expander;

public sealed record CoreForm(SchemeProcedure Procedure)
    : Transformer(Procedure);

public record Transformer(SchemeProcedure Procedure);