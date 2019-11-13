using Compiler.Model;


namespace Compiler.Model
{
    class Token
    {
        public LPD.Symbol Symbol;
        public string Lexeme;

        public Token(LPD.Symbol symbol, string lexeme)
        {
            Symbol = symbol;
            Lexeme = lexeme;
        }
    }
}
