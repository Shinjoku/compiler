using System.Collections.Generic;

namespace Compiler.Model
{
    public abstract class LPD
    {
        public enum Symbol
        {
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

        public static readonly Dictionary<string, Symbol> Keywords = new Dictionary<string, Symbol>()
        {
            ["programa"] = Symbol.PROGRAM,
            ["se"] = Symbol.IF,
            ["entao"] = Symbol.THEN,
            ["senao"] = Symbol.ELSE,
            ["enquanto"] = Symbol.WHILE,
            ["faca"] = Symbol.DO,
            ["inicio"] = Symbol.BEGIN,
            ["fim"] = Symbol.END,
            ["escreva"] = Symbol.WRITE,
            ["leia"] = Symbol.READ,
            ["var"] = Symbol.VARIABLE,
            ["inteiro"] = Symbol.INTEGER,
            ["booleano"] = Symbol.BOOLEAN,
            ["verdadeiro"] = Symbol.TRUE,
            ["falso"] = Symbol.FALSE,
            ["procedimento"] = Symbol.PROCEDURE,
            ["funcao"] = Symbol.FUNCTION,
            ["div"] = Symbol.DIVISION,
            ["e"] = Symbol.AND,
            ["ou"] = Symbol.OR,
            ["nao"] = Symbol.NOT
        };
    }
}
