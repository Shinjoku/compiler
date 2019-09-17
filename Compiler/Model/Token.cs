namespace Compiler.Model
{
    class Token
    {
        public int Symbol;
        public string Lexeme;

        public Token(int symbol, string lexeme)
        {
            Symbol = symbol;
            Lexeme = lexeme;
        }
    }
}
