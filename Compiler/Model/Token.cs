using System.Collections.Generic;

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
            WHILE,
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
            COLON,
            TRUE,
            FALSE,
        }

        public static readonly Dictionary<string, LPDSymbol> Keywords = new Dictionary<string, LPDSymbol>()
        {
            ["programa"] = LPDSymbol.PROGRAM,
            ["se"] = LPDSymbol.IF,
            ["entao"] = LPDSymbol.THEN,
            ["senao"] = LPDSymbol.ELSE,
            ["enquanto"] = LPDSymbol.WHILE,
            ["faca"] = LPDSymbol.DO,
            ["inicio"] = LPDSymbol.BEGIN,
            ["fim"] = LPDSymbol.END,
            ["escreva"] = LPDSymbol.WRITE,
            ["leia"] = LPDSymbol.READ,
            ["var"] = LPDSymbol.VARIABLE,
            ["inteiro"] = LPDSymbol.INTEGER,
            ["booleano"] = LPDSymbol.BOOLEAN,
            ["verdadeiro"] = LPDSymbol.TRUE,
            ["falso"] = LPDSymbol.FALSE,
            ["procedimento"] = LPDSymbol.PROCEDURE,
            ["funcao"] = LPDSymbol.FUNCTION,
            ["div"] = LPDSymbol.DIVISION,
            ["e"] = LPDSymbol.AND,
            ["ou"] = LPDSymbol.OR,
            ["nao"] = LPDSymbol.NOT
        };

        public int Symbol;
        public string Lexeme;

        public Token(int symbol, string lexeme)
        {
            Symbol = symbol;
            Lexeme = lexeme;
        }
    }
}
