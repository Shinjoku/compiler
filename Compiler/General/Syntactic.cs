using Compiler.Model;
using Compiler.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.General
{
    public class Syntactic : IAnalyzable
    {
        private Token _currentToken;
        private Lexical _lexical;
        private SymbolsTable _symbolsTable;
        private uint _scope;

        public Syntactic()
        {
            _scope = 0;
            _lexical = new Lexical();
            _symbolsTable = new SymbolsTable();
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
            if (_currentToken.Symbol == LPD.Symbol.PROGRAM)
            {
                _currentToken = _lexical.GetNextToken();
                if (_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
                {
                    _currentToken = _lexical.GetNextToken();
                    if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                    {
                        AnalyzeBlock();
                        if (_currentToken.Symbol != LPD.Symbol.DOT)
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
            if (_currentToken.Symbol == LPD.Symbol.BEGIN)
            {
                _currentToken = _lexical.GetNextToken();

                AnalyzeSimpleCommand();
                while (_currentToken.Symbol != LPD.Symbol.END)
                {
                    if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                    {
                        _currentToken = _lexical.GetNextToken();
                        if (_currentToken.Symbol != LPD.Symbol.END)
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

            if (_currentToken.Symbol == LPD.Symbol.THEN)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeSimpleCommand();
                if (_currentToken.Symbol == LPD.Symbol.ELSE)
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

            if (_currentToken.Symbol == LPD.Symbol.DO)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeSimpleCommand();
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeExpression()
        {
            AnalyzeSimpleExpression();

            if (_currentToken.Symbol == LPD.Symbol.GREATER ||
                _currentToken.Symbol == LPD.Symbol.GREATER_EQUAL ||
                _currentToken.Symbol == LPD.Symbol.LESSER ||
                _currentToken.Symbol == LPD.Symbol.LESSER_EQUAL ||
                _currentToken.Symbol == LPD.Symbol.EQUAL ||
                _currentToken.Symbol == LPD.Symbol.DIFFERENT)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeSimpleExpression();
            }
        }

        private void AnalyzeSimpleExpression()
        {

            if (_currentToken.Symbol == LPD.Symbol.PLUS ||
                _currentToken.Symbol == LPD.Symbol.MINUS)
            {
                // Identificou mais e menos unarios
                _currentToken = _lexical.GetNextToken();
            }

            AnalyzeTerm();

            while (_currentToken.Symbol == LPD.Symbol.PLUS ||
                _currentToken.Symbol == LPD.Symbol.MINUS ||
                _currentToken.Symbol == LPD.Symbol.OR)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeTerm();
            }
        }

        private void AnalyzeTerm()
        {
            AnalyzeFactor();
            while(_currentToken.Symbol == LPD.Symbol.MULTIPLICATION ||
                _currentToken.Symbol == LPD.Symbol.DIVISION||
                _currentToken.Symbol == LPD.Symbol.AND)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeFactor();
            }
        }

        private void AnalyzeFactor()
        {
            if (_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
            {
                Symbol symbol = _symbolsTable.GetSymbol(_currentToken.Lexeme);
                if (symbol == null)
                    throw new UndefinedIdentifierException(UndefinedIdentifierMessage("identifier"));

                if (symbol.IdentifierType == LPD.IdentifierType.FUNCTION)
                {
                    AnalyzeFunctionCall();
                }
                else
                {
                    // add variable to expression analyzer
                    _currentToken = _lexical.GetNextToken();
                }
            }
            else if (_currentToken.Symbol == LPD.Symbol.NUMBER)
                _currentToken = _lexical.GetNextToken();
            else if (_currentToken.Symbol == LPD.Symbol.NOT)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeFactor();
            }
            else if (_currentToken.Symbol == LPD.Symbol.OPEN_PARENTHESIS)
            {
                _currentToken = _lexical.GetNextToken();
                AnalyzeExpression();

                if (_currentToken.Symbol == LPD.Symbol.CLOSE_PARENTHESIS)
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

            //if (_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
            //{
            //    _currentToken = _lexical.GetNextToken();
            //    if (_currentToken.Symbol == LPD.Symbol.COLON)
            //    {
            //        _currentToken = _lexical.GetNextToken();
            //        if (_currentToken.Symbol == LPD.Symbol.INTEGER ||
            //            _currentToken.Symbol == LPD.Symbol.BOOLEAN)
            //        {
            //            _currentToken = _lexical.GetNextToken();
            //            if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
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

            if(_currentToken.Symbol == LPD.Symbol.OPEN_PARENTHESIS)
            {
                _currentToken = _lexical.GetNextToken();
                if(_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
                {
                    //SEMANTIC
                    if (_symbolsTable.IsAValidVariable(_currentToken.Lexeme))
                    {
                        _currentToken = _lexical.GetNextToken();
                        if (_currentToken.Symbol == LPD.Symbol.CLOSE_PARENTHESIS)
                            _currentToken = _lexical.GetNextToken();
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else throw new UndefinedIdentifierException(UndefinedIdentifierMessage("variable"));
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeRead()
        {
            _currentToken = _lexical.GetNextToken();

            if (_currentToken.Symbol == LPD.Symbol.OPEN_PARENTHESIS)
            {
                _currentToken = _lexical.GetNextToken();
                if (_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
                {
                    // SEMANTIC
                    if (_symbolsTable.IsAValidVariable(_currentToken.Lexeme))
                    {
                        _currentToken = _lexical.GetNextToken();

                        if (_currentToken.Symbol == LPD.Symbol.CLOSE_PARENTHESIS)
                            _currentToken = _lexical.GetNextToken();
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else throw new UndefinedIdentifierException(UndefinedIdentifierMessage("variable"));
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeAttributionOrProcedure()
        {
            Token oldToken = _currentToken;
            _currentToken = _lexical.GetNextToken();
            if (_currentToken.Symbol == LPD.Symbol.ATTRIBUTION)
                AnalyzeAttribution(oldToken);
            else
                AnalyzeProcedureCall(oldToken);
        }

        private void AnalyzeAttribution(Token variable)
        {
            if (!_symbolsTable.IsAValidVariable(variable.Lexeme))
                throw new UndefinedIdentifierException(UndefinedIdentifierMessage("variable", variable.Lexeme));
            _currentToken = _lexical.GetNextToken();
            AnalyzeExpression();
        }

        private void AnalyzeProcedureCall(Token procedure)
        {
            // SEMANTIC
            if (!_symbolsTable.IsAValidProcedure(procedure.Lexeme))
                throw new UndefinedIdentifierException(UndefinedIdentifierMessage("procedure", procedure.Lexeme));
            // --------

            // Code Generator CALL
        }

        private void AnalyzeSubroutines()
        {
            if(_currentToken.Symbol == LPD.Symbol.PROCEDURE ||
                _currentToken.Symbol == LPD.Symbol.FUNCTION)
            {
                // TODO: CodeGeneration
            }

            while (_currentToken.Symbol == LPD.Symbol.PROCEDURE ||
                _currentToken.Symbol == LPD.Symbol.FUNCTION)
            {
                if (_currentToken.Symbol == LPD.Symbol.PROCEDURE)
                    AnalyzeProcedureDeclaration();

                else if (_currentToken.Symbol == LPD.Symbol.FUNCTION)
                    AnalyzeFunctionDeclaration();

                else if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                    _currentToken = _lexical.GetNextToken();

                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            }
        }

        private void AnalyzeFunctionDeclaration()
        {
            _currentToken = _lexical.GetNextToken();

            if(_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
            {
                // SEMANTIC
                if(!_symbolsTable.IsAValidVariable(_currentToken.Lexeme))
                {
                    Token function = _currentToken;

                    _currentToken = _lexical.GetNextToken();

                    if (_currentToken.Symbol == LPD.Symbol.COLON)
                    {
                        _currentToken = _lexical.GetNextToken();
                        if (_currentToken.Symbol == LPD.Symbol.INTEGER ||
                            _currentToken.Symbol == LPD.Symbol.BOOLEAN)
                        {
                            //TODO: Should add the function type too
                            _symbolsTable.Add(new TypedSymbol(function.Lexeme, _scope, LPD.IdentifierType.FUNCTION));

                            _currentToken = _lexical.GetNextToken();
                            if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                            {
                                AnalyzeBlock();
                                if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                                {
                                    _currentToken = _lexical.GetNextToken();
                                }
                                else throw new UnexpectedTokenException(UnexpectedTokenMessage(";"));
                            }
                            
                        }
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                    }
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage(":"));
                }
                else throw new DuplicatedIdentifierException(DuplicatedIdentifierException(_currentToken.Lexeme));
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage());
        }

        private void AnalyzeProcedureDeclaration()
        {
            _currentToken = _lexical.GetNextToken();

            if (_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
            {
                // SEMANTIC
                if(!_symbolsTable.IsAValidProcedure(_currentToken.Lexeme)) {

                    _symbolsTable.Add(new Symbol(_currentToken.Lexeme, _scope, LPD.IdentifierType.PROCEDURE));
                    _scope += 1;

                    _currentToken = _lexical.GetNextToken();
                    if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                    {
                        AnalyzeBlock();
                        if (_currentToken.Symbol == LPD.Symbol.SEMICOLON)
                        {
                            _currentToken = _lexical.GetNextToken();
                        }
                        else throw new UnexpectedTokenException(UnexpectedTokenMessage(";"));
                    }
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage(";"));
                }
            }
            else throw new UnexpectedTokenException(UnexpectedTokenMessage("identifier"));

            _symbolsTable.RemoveUntil(_scope++);
        }

        private void AnalyzeVariablesStage()
        {
            // This case is optional
            if (_currentToken.Symbol == LPD.Symbol.VARIABLE)
            {
                
                _currentToken = _lexical.GetNextToken();
                if(_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
                {
                    while(_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
                    {
                        AnalyzeVariables();
                        if(_currentToken.Symbol == LPD.Symbol.SEMICOLON)
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
                if (_currentToken.Symbol == LPD.Symbol.IDENTIFIER)
                {
                    variables.Add(_currentToken);
                    _currentToken = _lexical.GetNextToken();
                    if(_currentToken.Symbol == LPD.Symbol.COMMA ||
                        _currentToken.Symbol == LPD.Symbol.COLON)
                    {
                        if(_currentToken.Symbol == LPD.Symbol.COMMA)
                        {
                            _currentToken = _lexical.GetNextToken();
                            if(_currentToken.Symbol == LPD.Symbol.COLON)
                                throw new UnexpectedTokenException(UnexpectedTokenMessage());
                        }
                        // Else is colon
                    }
                    else throw new UnexpectedTokenException(UnexpectedTokenMessage());
                }
                else throw new UnexpectedTokenException(UnexpectedTokenMessage());
            } while (_currentToken.Symbol != LPD.Symbol.COLON);

            _currentToken = _lexical.GetNextToken();
            LPD.ValueType valueType = AnalyzeValueType();

            foreach (var var in variables)
                _symbolsTable.Add(new TypedSymbol(var.Lexeme, _scope, LPD.IdentifierType.VARIABLE, valueType));
        }

        private LPD.ValueType AnalyzeValueType()
        {
            LPD.ValueType type;
            LPD.Symbol symbol = _currentToken.Symbol;

            if (symbol != LPD.Symbol.INTEGER && symbol != LPD.Symbol.BOOLEAN)
                throw new UnexpectedTokenException(UnexpectedTokenMessage());
            else
            {
                if (symbol == LPD.Symbol.INTEGER)
                    type = LPD.ValueType.INTEGER;
                else
                    type = LPD.ValueType.BOOLEAN;
            }

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

        private string UnexpectedTokenMessage(string expectedToken)
        {
            return "Unexpected token '" + _currentToken.Lexeme +
                    "' at line " + _lexical.Position.Line +
                    ", column " + _lexical.Position.Column +
                    ". Expected '" + expectedToken + "'";
        }

        private string UndefinedIdentifierMessage(string type)
        {
            return "Undefined " + type + " '" + _currentToken.Lexeme +
                    "' at line " + _lexical.Position.Line +
                    ", column " + _lexical.Position.Column;
        }

        private string UndefinedIdentifierMessage(string type, string lexeme)
        {
            return "Undefined " + type + " '" + lexeme +
                    "' at line " + _lexical.Position.Line +
                    ", column " + _lexical.Position.Column;
        }

        private string DuplicatedIdentifierException(string identifierType)
        {
            return "Duplicated " + identifierType + " '" + _currentToken.Lexeme +
                    "' at line " + _lexical.Position.Line +
                    ", column " + _lexical.Position.Column;
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

    class DuplicatedIdentifierException : Exception
    {
        public DuplicatedIdentifierException() : base() { }
        public DuplicatedIdentifierException(string message) : base(message) { }
        public DuplicatedIdentifierException(string message, Exception inner) : base(message, inner) { }
    }
}
