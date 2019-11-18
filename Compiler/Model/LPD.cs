using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compiler.Model
{
    public static class LPD
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
            UNARYPLUS,
            UNARYMINUS
        }

        public enum ValueType
        {
            INTEGER,
            BOOLEAN,
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
            ["BOOLEANo"] = Symbol.BOOLEAN,
            ["verdadeiro"] = Symbol.TRUE,
            ["falso"] = Symbol.FALSE,
            ["procedimento"] = Symbol.PROCEDURE,
            ["funcao"] = Symbol.FUNCTION,
            ["div"] = Symbol.DIVISION,
            ["e"] = Symbol.AND,
            ["ou"] = Symbol.OR,
            ["nao"] = Symbol.NOT
        };

        public static readonly Dictionary<Symbol, string> IdentifierTypes = new Dictionary<Symbol, string>()
        {
            [Symbol.VARIABLE] = "Variable",
            [Symbol.PROCEDURE] = "Procedure",
            [Symbol.FUNCTION] = "Function",
        };

        public static readonly Dictionary<Symbol, int> OperatorsPriority = new Dictionary<Symbol, int>()
        {
            [Symbol.NOT] = 5,
            [Symbol.UNARYPLUS] = 5,
            [Symbol.UNARYMINUS] = 5,
            [Symbol.MULTIPLICATION] = 4,
            [Symbol.DIVISION] = 4,
            [Symbol.PLUS] = 3,
            [Symbol.MINUS] = 3,
            [Symbol.GREATER] = 2,
            [Symbol.GREATER_EQUAL] = 2,
            [Symbol.LESSER] = 2,
            [Symbol.LESSER_EQUAL] = 2,
            [Symbol.DIFFERENT] = 2,
            [Symbol.EQUAL] = 2,
            [Symbol.AND] = 1,
            [Symbol.OR] = 0,
        };

        public static Dictionary<Symbol, ValueType> OperatorsType = new Dictionary<Symbol, ValueType>
        {
            [Symbol.UNARYPLUS] = ValueType.INTEGER,
            [Symbol.UNARYMINUS] = ValueType.INTEGER,
            [Symbol.MULTIPLICATION] = ValueType.INTEGER,
            [Symbol.DIVISION] = ValueType.INTEGER,
            [Symbol.PLUS] = ValueType.INTEGER,
            [Symbol.MINUS] = ValueType.INTEGER,
            [Symbol.NOT] = ValueType.BOOLEAN,
            [Symbol.GREATER] = ValueType.BOOLEAN,
            [Symbol.GREATER_EQUAL] = ValueType.BOOLEAN,
            [Symbol.LESSER] = ValueType.BOOLEAN,
            [Symbol.LESSER_EQUAL] = ValueType.BOOLEAN,
            [Symbol.DIFFERENT] = ValueType.BOOLEAN,
            [Symbol.EQUAL] = ValueType.BOOLEAN,
            [Symbol.AND] = ValueType.BOOLEAN,
            [Symbol.OR] = ValueType.BOOLEAN,
            [Symbol.NUMBER] = ValueType.INTEGER,
            [Symbol.TRUE] = ValueType.BOOLEAN,
            [Symbol.FALSE] = ValueType.BOOLEAN
        };

        #region Characters Lists

        public static readonly Regex Letters = new Regex(@"[a-zA-Z_]");
        public static readonly Regex Digits = new Regex(@"[0-9]");
        public static readonly Regex ArithmeticOperators = new Regex(@"[+\-*]");
        public static readonly Regex RelationalOperators = new Regex(@"[<>=!]");
        public static readonly Regex PunctuationCharacters = new Regex(@"[;,.()]");
        public static readonly Regex SpaceCharacters = new Regex("[ \\t\\n\\r\\0]");

        #endregion
    }
}
