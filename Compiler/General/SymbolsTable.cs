using System.Linq;
using Compiler.Model;
using System.Collections.Generic;


namespace Compiler.General
{
    class SymbolsTable
    {
        private List<Symbol> _symbolsList;

        public SymbolsTable()
        {
            _symbolsList = new List<Symbol>();
        }

        public SymbolsTable(IEnumerable<Symbol> symbolsList)
        {
            _symbolsList = new List<Symbol>(symbolsList);
        }

        public void Add(Symbol symbol)
        {
            _symbolsList.Add(symbol);
        }

        public void RemoveUntil(uint scope)
        {
            _symbolsList = _symbolsList.Where(s => s.Scope < scope).ToList();
        }

        public Symbol GetSymbol(string lexeme)
        {
            return _symbolsList.Where(s => s.Label == lexeme).LastOrDefault();
        }

        public bool IsAValidVariable(string lexeme)
        {
            var variable = _symbolsList
                .Select(el => el)
                .Where(el => el.IdentifierType == LPD.IdentifierType.VARIABLE && el.Label == lexeme)
                .FirstOrDefault();
            return variable?.Label.Equals(lexeme) ?? false;
        }

        public bool IsAValidProcedure(string lexeme)
        {
            var procedure = _symbolsList
                .Select(el => el)
                .Where(el => el.IdentifierType == LPD.IdentifierType.PROCEDURE && el.Label == lexeme)
                .FirstOrDefault();
            return procedure?.Label.Equals(lexeme) ?? false;
        }

        public bool IsAValidFunction(string lexeme)
        {
            var function = _symbolsList
                .Select(el => el)
                .Where(el => el.IdentifierType == LPD.IdentifierType.FUNCTION && el.Label == lexeme)
                .FirstOrDefault();
            return function?.Label.Equals(lexeme) ?? false;
        }

    }
}
