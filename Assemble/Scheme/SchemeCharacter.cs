namespace Assemble.Scheme;

public sealed class SchemeCharacter : SchemeDatum
{
    public static class Names
    {
        private static readonly Dictionary<string, char> _nameToCharLookup = new()
        {
            { Alarm, AlarmChar },
            { Backspace, BackspaceChar},
            { Delete, DeleteChar },
            { Escape, EscapeChar },
            { NewLine, NewLineChar },
            { Null, NullChar },
            { Return, ReturnChar },
            { Space, SpaceChar },
            { Tab, TabChar }
        };

        public static string? Lookup(char c)
        {
            return c switch
            {
                AlarmChar => Alarm,
                BackspaceChar => Backspace,
                DeleteChar => Delete,
                EscapeChar => Escape,
                NewLineChar => NewLine,
                NullChar => Null,
                ReturnChar => Return,
                SpaceChar => Space,
                TabChar => Tab,
                _ => null,
            };
        }
        public static char? Lookup(string name)
        {
            return _nameToCharLookup.GetValueOrDefault(name);
        }

        public const string Alarm = "alarm";
        public const char AlarmChar = '\u0007';

        public const string Backspace = "backspace";
        public const char BackspaceChar = '\u0008';

        public const string Delete = "delete";
        public const char DeleteChar = '\u007F';

        public const string Escape = "escape";
        public const char EscapeChar = '\u001B';

        public const string NewLine = "newline";
        public const char NewLineChar = '\u000A';

        public const string Null = "null";
        public const char NullChar = '\u0000';

        public const string Return = "return";
        public const char ReturnChar = '\u000D';

        public const string Space = "space";
        public const char SpaceChar = '\u0020';

        public const string Tab = "tab";
        public const char TabChar = '\u0009';
    }

    public SchemeCharacter(char value)
    {
        Value = value;
    }

    public char Value { get; init; }

    public override string Name => "character";

    public override bool Equals(SchemeObject? other)
        => other is not null && other is SchemeCharacter b && b.Value == Value;

    public override bool Same(SchemeObject other)
        => Equals(other);

    public override string Write()
    {
        const string prefix = "#\\";

        var name = Names.Lookup(Value);
        if (name is not null)
            return prefix + name;

        if (char.IsAsciiLetterOrDigit(Value))
            return prefix + Value;

        return prefix + "x" + Convert.ToByte(Value).ToString("x2");
    }
}
