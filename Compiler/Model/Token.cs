using Compiler.Model;


namespace Compiler.Model
{
    public class Token
    {
        public LPD.Symbol Symbol { get; set; }
        public string Lexeme { get; set; }
        public uint Scope { get; set; }
        public LPD.Symbol ReturnType { get; set; }


        public Token(LPD.Symbol symbol, string lexeme)
        {
            Symbol = symbol;
            Lexeme = lexeme;
        }

        public Token(LPD.Symbol symbol, string lexeme, uint scope)
        {
            Symbol = symbol;
            Lexeme = lexeme;
            Scope = scope;
        }

        public Token(LPD.Symbol symbol, string lexeme, uint scope, LPD.Symbol returnType)
        {
            Symbol = symbol;
            Lexeme = lexeme;
            Scope = scope;
            ReturnType = returnType;
        }
    }
}
