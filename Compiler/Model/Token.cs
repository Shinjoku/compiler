namespace Compiler.Model
{
    class Token
    {
        public enum LPDSymbol {
            PROGRAM,
            BEGIN,
            END,
            PROCEDURE,
            FUNCTION,
            IF,
            THEN,
            ELSE,
            DO,
            ATTRIBUTION,
            WRITE,
            READ,
            VARIABLE,
            INTEGER,
            BOOLEAN,
            IDENTIFIER,
            NUMBER,
            DOT,
            SEMICOLON,
            COMMA,
            OPEN_PARENTHESIS,
            CLOSE_PARENTHESIS,
            GREATER,
            GREATER_EQUAL,
            EQUAL,
            LESSER,
            LESSER_EQUAL,
            DIFFERENT,
            PLUS,
            MINUS,
            MULTIPLICATION,
            DIVISION,
            AND,
            OR,
            NOT,
            COLON
        }

        public int Symbol;
        public string Lexeme;
    }
}
