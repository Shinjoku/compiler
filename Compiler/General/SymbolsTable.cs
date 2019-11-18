using System.Linq;
using Compiler.Model;
using System.Collections.Generic;


namespace Compiler.General
{
    class SymbolsTable
    {
        private List<Token> _symbolsList;

        public SymbolsTable()
        {
            _symbolsList = new List<Token>();
        }

        public SymbolsTable(IEnumerable<Token> symbolsList)
        {
            _symbolsList = new List<Token>(symbolsList);
        }

        public void Add(Token symbol)
        {
            _symbolsList.Add(symbol);
        }

        public void RemoveUntil(uint scope)
        {
            _symbolsList = _symbolsList.Where(s => s.Scope < scope).ToList();
        }

        public Token GetSymbol(string lexeme)
        {
            return _symbolsList.Where(s => s.Lexeme == lexeme).LastOrDefault();
        }

        public bool IsAValidVariable(string lexeme)
        {
            var variable = _symbolsList
                .Select(el => el)
                .Where(el => el.Symbol == LPD.Symbol.VARIABLE && el.Lexeme == lexeme)
                .FirstOrDefault();
            return variable?.Lexeme.Equals(lexeme) ?? false;
        }

        public bool IsAValidProcedure(string lexeme)
        {
            var procedure = _symbolsList
                .Select(el => el)
                .Where(el => el.Symbol == LPD.Symbol.PROCEDURE && el.Lexeme == lexeme)
                .FirstOrDefault();
            return procedure?.Lexeme.Equals(lexeme) ?? false;
        }

        public bool IsAValidFunction(string lexeme)
        {
            var function = _symbolsList
                .Select(el => el)
                .Where(el => el.Symbol == LPD.Symbol.FUNCTION && el.Lexeme == lexeme)
                .FirstOrDefault();
            return function?.Lexeme.Equals(lexeme) ?? false;
        }

    }
}
