using Compiler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.General
{
    public class Syntactic
    {
        private Token _currentToken;
        private Lexical _lexical;
        protected List<Symbol> SymbolsTable;

        public Syntactic()
        {
            _lexical = new Lexical();
            SymbolsTable = new List<Symbol>();
        }

        public bool Run(string filePath)
        {
            _lexical.OpenFile(filePath);

            try
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeProgram();
                return true;
            }
            catch (UnexpectedTokenException e)
            {
                throw e;
            }
            catch (UndefinedIdentifierException ex)
            {
                throw ex;
            }
            finally
            {
                _lexical.CloseFile();
            }
        }

        #region Analysis

        private void AnalyzeProgram()
        {
            if (_currentToken.Symbol == (int)LPD.Symbol.PROGRAM)
            {
                _currentToken = _lexical.GetNextToken();
                if (_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
                {
                    _currentToken = _lexical.GetNextToken();
                    if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                    {
                        AnalyzeBlock();
                        if (_currentToken.Symbol != (int)LPD.Symbol.DOT)
                            throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else
                        throw new UnexpectedTokenException(UnexpectedTokenMessage());
                }
                else
                    throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else
                throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeBlock()
        {
            _currentToken = _lexical.GetNextToken();
            AnalyzeVariablesStage();
            AnalyzeSubroutines();
            AnalyzeCommands();
        }

        private void AnalyzeCommands()
        {
            if (_currentToken.Symbol == (int)LPD.Symbol.BEGIN)
            {
                _currentToken = _lexical.GetNextToken();

                AnalyzeSimpleCommand();
                while (_currentToken.Symbol != (int)LPD.Symbol.END)
                {
                    if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                    {
                        _currentToken = _lexical.GetNextToken();
                        if (_currentToken.Symbol != (int)LPD.Symbol.END)
                        {
                            AnalyzeSimpleCommand();
                        }
                    }
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                }
                // Is Symbol END, should get the next token
                _currentToken = _lexical.GetNextToken();
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeSimpleCommand()
        {
            switch ((LPD.Symbol)_currentToken.Symbol)
            {
                case LPD.Symbol.IDENTIFIER:
                    AnalyzeAttributionOrProcedure();
                    break;
                case LPD.Symbol.IF:
                    AnalyzeIf();
                    break;
                case LPD.Symbol.WHILE:
                    AnalyzeWhile();
                    break;
                case LPD.Symbol.READ:
                    AnalyzeRead();
                    break;
                case LPD.Symbol.WRITE:
                    AnalyzeWrite();
                    break;
                default:
                    AnalyzeCommands();
                    break;
            }
        }

        private void AnalyzeIf()
        {
            _currentToken = _lexical.GetNextToken();
            AnalyzeExpression();

            if (_currentToken.Symbol == (int)LPD.Symbol.THEN)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeSimpleCommand();
                if (_currentToken.Symbol == (int)LPD.Symbol.ELSE)
                {
                    _currentToken = _lexical.GetNextToken();
                    AnalyzeSimpleCommand();
                }
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeWhile()
        {
            _currentToken = _lexical.GetNextToken();
            AnalyzeExpression();

            if (_currentToken.Symbol == (int)LPD.Symbol.DO)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeSimpleCommand();
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeExpression()
        {
            AnalyzeSimpleExpression();

            if (_currentToken.Symbol == (int)LPD.Symbol.GREATER ||
                _currentToken.Symbol == (int)LPD.Symbol.GREATER_EQUAL ||
                _currentToken.Symbol == (int)LPD.Symbol.LESSER ||
                _currentToken.Symbol == (int)LPD.Symbol.LESSER_EQUAL ||
                _currentToken.Symbol == (int)LPD.Symbol.EQUAL ||
                _currentToken.Symbol == (int)LPD.Symbol.DIFFERENT)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeSimpleExpression();
            }
        }

        private void AnalyzeSimpleExpression()
        {
            AnalyzeTerm();

            if (_currentToken.Symbol == (int)LPD.Symbol.PLUS ||
                _currentToken.Symbol == (int)LPD.Symbol.MINUS ||
                _currentToken.Symbol == (int)LPD.Symbol.OR ||
                _currentToken.Symbol == (int)LPD.Symbol.GREATER ||
                _currentToken.Symbol == (int)LPD.Symbol.GREATER_EQUAL ||
                _currentToken.Symbol == (int)LPD.Symbol.LESSER ||
                _currentToken.Symbol == (int)LPD.Symbol.LESSER_EQUAL ||
                _currentToken.Symbol == (int)LPD.Symbol.EQUAL ||
                _currentToken.Symbol == (int)LPD.Symbol.DIFFERENT)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeTerm();

                while (_currentToken.Symbol == (int)LPD.Symbol.PLUS ||
                        _currentToken.Symbol == (int)LPD.Symbol.MINUS ||
                        _currentToken.Symbol == (int)LPD.Symbol.OR ||
                        _currentToken.Symbol == (int)LPD.Symbol.GREATER ||
                        _currentToken.Symbol == (int)LPD.Symbol.GREATER_EQUAL ||
                        _currentToken.Symbol == (int)LPD.Symbol.LESSER ||
                        _currentToken.Symbol == (int)LPD.Symbol.LESSER_EQUAL ||
                        _currentToken.Symbol == (int)LPD.Symbol.EQUAL ||
                        _currentToken.Symbol == (int)LPD.Symbol.DIFFERENT)
                {
                    _currentToken = _lexical.GetNextToken();
                    AnalyzeTerm();
                }
            }
        }

        private void AnalyzeTerm()
        {
            AnalyzeFactor();
            while(_currentToken.Symbol == (int)LPD.Symbol.MULTIPLICATION ||
                _currentToken.Symbol == (int)LPD.Symbol.DIVISION||
                _currentToken.Symbol == (int)LPD.Symbol.AND)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeFactor();
            }
        }

        private void AnalyzeFactor()
        {
            if (_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
            {
                if (IsAValidIdentifier(_currentToken.Lexeme))
                {
                    AnalyzeFunctionCall();
                }
                else throw new UndefinedIdentifierException(UndefinedIdentifierMessage());
            }
            else if (_currentToken.Symbol == (int)LPD.Symbol.NUMBER)
                _currentToken = _lexical.GetNextToken();
            else if (_currentToken.Symbol == (int)LPD.Symbol.NOT)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeFactor();
            }
            else if (_currentToken.Symbol == (int)LPD.Symbol.OPEN_PARENTHESIS)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeExpression();

                if (_currentToken.Symbol == (int)LPD.Symbol.CLOSE_PARENTHESIS)
                    _currentToken = _lexical.GetNextToken();

                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else if (_currentToken.Lexeme == "verdadeiro" ||
                _currentToken.Lexeme == "falso")
                _currentToken = _lexical.GetNextToken();
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeFunctionCall()
        {
            _currentToken = _lexical.GetNextToken();
            // TODO
            //if (_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
            //{
            //    _currentToken = _lexical.GetNextToken();
            //    if (_currentToken.Symbol == (int)LPD.Symbol.COLON)
            //    {
            //        _currentToken = _lexical.GetNextToken();
            //        if (_currentToken.Symbol == (int)LPD.Symbol.INTEGER ||
            //            _currentToken.Symbol == (int)LPD.Symbol.BOOLEAN)
            //        {
            //            _currentToken = _lexical.GetNextToken();
            //            if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
            //            {
            //                AnalyzeBlock();
            //            }
            //            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            //        }
            //        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            //    }
            //    else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            //}
            //else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeWrite()
        {
            _currentToken = _lexical.GetNextToken();

            if(_currentToken.Symbol == (int)LPD.Symbol.OPEN_PARENTHESIS)
            {
                _currentToken = _lexical.GetNextToken();
                if(_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
                {
                    if (IsAValidIdentifier(_currentToken.Lexeme))
                    {
                        _currentToken = _lexical.GetNextToken();
                        if (_currentToken.Symbol == (int)LPD.Symbol.CLOSE_PARENTHESIS)
                            _currentToken = _lexical.GetNextToken();
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else throw new UndefinedIdentifierException(UndefinedIdentifierMessage());
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeRead()
        {
            _currentToken = _lexical.GetNextToken();

            if (_currentToken.Symbol == (int)LPD.Symbol.OPEN_PARENTHESIS)
            {
                _currentToken = _lexical.GetNextToken();
                if (_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
                {
                    if (IsAValidIdentifier(_currentToken.Lexeme))
                    {
                        _currentToken = _lexical.GetNextToken();

                        if (_currentToken.Symbol == (int)LPD.Symbol.CLOSE_PARENTHESIS)
                            _currentToken = _lexical.GetNextToken();
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else throw new UndefinedIdentifierException(UndefinedIdentifierMessage());
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeAttributionOrProcedure()
        {
            _currentToken = _lexical.GetNextToken();
            if (_currentToken.Symbol == (int)LPD.Symbol.ATTRIBUTION)
                AnalyzeAttribution();
            else
                AnalyzeProcedureCall();
        }

        private void AnalyzeAttribution()
        {
            _currentToken = _lexical.GetNextToken();
            AnalyzeExpression();
        }

        private void AnalyzeProcedureCall()
        {
            //_currentToken = _lexical.GetNextToken();
        }

        private void AnalyzeSubroutines()
        {
            if(_currentToken.Symbol == (int)LPD.Symbol.PROCEDURE ||
                _currentToken.Symbol == (int)LPD.Symbol.FUNCTION)
            {
                // TODO: CodeGeneration
            }

            while (_currentToken.Symbol == (int)LPD.Symbol.PROCEDURE ||
                _currentToken.Symbol == (int)LPD.Symbol.FUNCTION)
            {
                if (_currentToken.Symbol == (int)LPD.Symbol.PROCEDURE)
                    AnalyzeProcedureDeclaration();

                else if (_currentToken.Symbol == (int)LPD.Symbol.FUNCTION)
                    AnalyzeFunctionDeclaration();

                else if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                    _currentToken = _lexical.GetNextToken();

                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
        }

        private void AnalyzeFunctionDeclaration()
        {
            _currentToken = _lexical.GetNextToken();

            if(_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
            {
                if(!IsAValidIdentifier(_currentToken.Lexeme)) {
                    Token function = _currentToken;

                    _currentToken = _lexical.GetNextToken();
                    if(_currentToken.Symbol == (int)LPD.Symbol.COLON)
                    {
                        _currentToken = _lexical.GetNextToken();
                        if (_currentToken.Symbol == (int)LPD.Symbol.INTEGER ||
                            _currentToken.Symbol == (int)LPD.Symbol.BOOLEAN)
                        {
                            // TODO: Should add the function type too
                            SymbolsTable.Add(new Symbol(function.Lexeme, 0, 0, false));

                            _currentToken = _lexical.GetNextToken();
                            if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                                AnalyzeBlock();
                        }
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeProcedureDeclaration()
        {
            _currentToken = _lexical.GetNextToken();

            if (_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
            {
                if(!IsAValidIdentifier(_currentToken.Lexeme)) {

                    SymbolsTable.Add(new Symbol(_currentToken.Lexeme, 0, 0, false));

                    _currentToken = _lexical.GetNextToken();
                    if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                    {
                        AnalyzeBlock();
                        if (_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                            _currentToken = _lexical.GetNextToken();
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                        
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                }
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeVariablesStage()
        {
            // This case is optional
            if (_currentToken.Symbol == (int)LPD.Symbol.VARIABLE)
            {
                
                _currentToken = _lexical.GetNextToken();
                if(_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
                {
                    while(_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
                    {
                        AnalyzeVariables();
                        if(_currentToken.Symbol == (int)LPD.Symbol.SEMICOLON)
                        {
                            _currentToken = _lexical.GetNextToken();
                        }
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
        }

        private void AnalyzeVariables()
        {
            var variables = new List<Token>();

            do
            {
                if (_currentToken.Symbol == (int)LPD.Symbol.IDENTIFIER)
                {
                    variables.Add(_currentToken);
                    _currentToken = _lexical.GetNextToken();
                    if(_currentToken.Symbol == (int)LPD.Symbol.COMMA ||
                        _currentToken.Symbol == (int)LPD.Symbol.COLON)
                    {
                        if(_currentToken.Symbol == (int)LPD.Symbol.COMMA)
                        {
                            _currentToken = _lexical.GetNextToken();
                            if(_currentToken.Symbol == (int)LPD.Symbol.COLON)
                                throw new UnexpectedTokenException(UnexpectedTokenMessage());
                        }
                        // Else is colon
                    }
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            } while (_currentToken.Symbol != (int)LPD.Symbol.COLON);

            _currentToken = _lexical.GetNextToken();
            int type = AnalyzeType();

            foreach (var var in variables)
                SymbolsTable.Add(new Symbol(var.Lexeme, type, 0, true));
        }

        private int AnalyzeType()
        {
            int type = 0;
            if (_currentToken.Symbol != (int)LPD.Symbol.INTEGER &&
                _currentToken.Symbol != (int)LPD.Symbol.BOOLEAN)
                throw new UnexpectedTokenException(UnexpectedTokenMessage());
            else type = _currentToken.Symbol;

            _currentToken = _lexical.GetNextToken();
            return type;
        }

        #endregion

        #region Aux functions

        private string UnexpectedTokenMessage()
        {
            return "Unexpected token '" + _currentToken.Lexeme +
                    "' at line " + _lexical.Position.Line +
                    ", column " + _lexical.Position.Column;
        }
        private string UndefinedIdentifierMessage()
        {
            return "Undefined variable '" + _currentToken.Lexeme +
                    "' at line " + _lexical.Position.Line +
                    ", column " + _lexical.Position.Column;
        }

        private bool IsAValidIdentifier(string lexeme)
        {
            var symbolList = SymbolsTable.Select(el => el.Label);
            return symbolList.Contains(lexeme);
        }

        #endregion
    }

    class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException() : base() { }
        public UnexpectedTokenException(string message) : base(message) { }
        public UnexpectedTokenException(string message, Exception inner) : base(message, inner) { }
    }

    class UndefinedIdentifierException : Exception
    {
        public UndefinedIdentifierException() : base() { }
        public UndefinedIdentifierException(string message) : base(message) { }
        public UndefinedIdentifierException(string message, Exception inner) : base(message, inner) { }
    }
}
