using Compiler.Model;
using Compiler.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.General
{
    class Lexical : IAnalyzable
    {
        private StreamReader _file;
        private List<Token> _tokens;
        private char _currentCharacter;

        public bool ReachedEndOfFile;
        public PointerPosition Position;

        public Lexical()
        {
            _tokens = new List<Token>();
        }

        public void OpenFile(string filePath)
        {
            _file = new StreamReader(filePath, Encoding.UTF8);
            Position = new PointerPosition();
        }

        public char GetNextChar()
        {
            try
            {
                var result = (char)_file.Read();
                Position.Column++;

                if (result == '\n')
                {
                    Position.Column = 1;
                    Position.Line++;
                }

                if(result == ushort.MaxValue) ReachedEndOfFile = true;
                return result;
            }
            catch (IOException)
            {
                ReachedEndOfFile = true;
                return (char)0;
            }
        }

        private void PassComment()
        {
            while (_currentCharacter != '}' && !ReachedEndOfFile)
            {
                _currentCharacter = GetNextChar();
            }

            // Current char is '}', so let's get another _currentCharacter
            _currentCharacter = GetNextChar();
        }

        private void PassSpace()
        {
            while (LPD.SpaceCharacters.IsMatch(_currentCharacter.ToString()) && !ReachedEndOfFile)
            {
                _currentCharacter = GetNextChar();
            }
        }

        public void CloseFile()
        {
            _file.Close();
        }

        /// <summary>
        /// For debug purposes.
        /// </summary>
        /// <param name="filePath">File path that will be used for the compilation.</param>
        /// <returns></returns>
        public bool Run(string filePath)
        {
            try
            {
                OpenFile(filePath);
                _currentCharacter = GetNextChar();

                while (!ReachedEndOfFile)
                {
                    while((_currentCharacter == '{' || LPD.SpaceCharacters.IsMatch(_currentCharacter.ToString())) &&
                        !ReachedEndOfFile)
                    {
                        if (_currentCharacter == '{')
                            PassComment();

                        PassSpace();
                    }

                    if (!ReachedEndOfFile)
                        _tokens.Add(GetNextToken());
                }

                return true;
            }
            catch (NotSupportedCharacterException e)
            {
                throw new NotSupportedCharacterException(e.Message);
            }
            finally
            {
                CloseFile();
            }
        }

        private void PassCommentAndSpaces()
        {
            while ((_currentCharacter == '{' || LPD.SpaceCharacters.IsMatch(_currentCharacter.ToString())) &&
                !ReachedEndOfFile)
            {
                if (_currentCharacter == '{')
                    PassComment();

                PassSpace();
            }
        }

        public Token GetNextToken()
        {
            try
            {
                PassCommentAndSpaces();

                var currChar = _currentCharacter.ToString();

                if (LPD.Digits.IsMatch(currChar))
                    return HandleNumber();

                else if (LPD.Letters.IsMatch(currChar))
                    return HandleIdentifierAndKeyWord();

                else if (_currentCharacter == ':')
                    return HandleAttribution();

                else if (LPD.ArithmeticOperators.IsMatch(currChar))
                    return HandleArithmeticOperator();

                else if (LPD.RelationalOperators.IsMatch(currChar))
                    return HandleRelationalOperators();

                else if (LPD.PunctuationCharacters.IsMatch(currChar))
                    return HandlePunctuation();

                else throw new NotSupportedCharacterException(
                    "Not supported character '" + _currentCharacter +
                    "' on line " + Position.Line +
                    ", at column " + Position.Column + ".");
            }
            catch (NotSupportedCharacterException e)
            {
                throw new NotSupportedCharacterException(e.Message);
            }
        }

        private Token HandlePunctuation()
        {
            Token result;
            string symbol = _currentCharacter.ToString();

            switch (_currentCharacter)
            {
                case ';':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result =  new Token(LPD.Symbol.SEMICOLON, symbol);
                    break;

                case '.': 
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token(LPD.Symbol.DOT, symbol);
                    break;

                case '(':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token(LPD.Symbol.OPEN_PARENTHESIS, symbol);
                    break;

                case ')':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token(LPD.Symbol.CLOSE_PARENTHESIS, symbol);
                    break;
                case ',':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token(LPD.Symbol.COMMA, symbol);
                    break;

                default:
                    Console.WriteLine("PUNCTUATION TOKEN CANNOT BE FOUND!");
                    return null;
            }

            _currentCharacter = GetNextChar();
            return result;
        }

        private Token HandleRelationalOperators()
        {
            string op = "" + _currentCharacter;
            Token result;

            switch (op)
            {
                case ">":
                    _currentCharacter = GetNextChar();
                    if (_currentCharacter == '=')
                    {
                        op += _currentCharacter;
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token(LPD.Symbol.GREATER_EQUAL, op);
                        _currentCharacter = GetNextChar();
                    }
                    else
                    {
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token(LPD.Symbol.GREATER, op);
                    }
                    break;

                case "<":
                    _currentCharacter = GetNextChar();
                    if (_currentCharacter == '=')
                    {
                        op += _currentCharacter;
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token(LPD.Symbol.LESSER_EQUAL, op);
                        _currentCharacter = GetNextChar();
                    }
                    else
                    {
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token(LPD.Symbol.LESSER, op);
                    }
                    break;

                case "=":
                    Console.WriteLine("Relational Operator: {0}", op);
                    result = new Token(LPD.Symbol.EQUAL, op);
                    break;

                case "!":
                    var lastChar = _currentCharacter;
                    _currentCharacter = GetNextChar();
                    if (_currentCharacter == '=')
                    {
                        op += _currentCharacter;
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token(LPD.Symbol.DIFFERENT, op);
                        _currentCharacter = GetNextChar();
                    }
                    else throw new NotSupportedCharacterException(
                        "Not supported character '" + lastChar +
                        "' on line " + Position.Line +
                        ", at column " + Position.Column + "." +
                        " '!=' was expected.");
                    break;

                default:
                    Console.WriteLine("RELATIONAL TOKEN CANNOT BE FOUND!");
                    return null;
            }

            return result;
        }

        private Token HandleArithmeticOperator()
        {
            string op = _currentCharacter.ToString();
            Token result;

            switch (_currentCharacter)
            {
                case '+':
                    Console.WriteLine("Arithmetic Operator: {0}", op);
                    result = new Token(LPD.Symbol.PLUS, op);
                    break;

                case '-':
                    Console.WriteLine("Arithmetic Operator: {0}", op);
                    result = new Token(LPD.Symbol.MINUS, op);
                    break;

                case '*':
                    Console.WriteLine("Arithmetic Operator: {0}", op);
                    result = new Token(LPD.Symbol.MULTIPLICATION, op);
                    break;

                default:
                    Console.WriteLine("ARITHMETIC TOKEN CANNOT BE FOUND!");
                    return null;
            }

            _currentCharacter = GetNextChar();
            return result;
        }

        private Token HandleAttribution()
        {
            string characters = "" + _currentCharacter;

            _currentCharacter = GetNextChar();
            if(_currentCharacter != '=')
            {
                Console.WriteLine("Attribution: {0}", characters);
                return new Token(LPD.Symbol.COLON, characters);
            }
            else
            {
                characters += _currentCharacter;
                Console.WriteLine("Attribution: {0}", characters);
                _currentCharacter = GetNextChar();
                return new Token(LPD.Symbol.ATTRIBUTION, characters);
            }
        }

        private Token HandleIdentifierAndKeyWord()
        {
            var identifierSB = new StringBuilder();

            identifierSB.Append(_currentCharacter);
            _currentCharacter = GetNextChar();

            while (LPD.Letters.IsMatch(_currentCharacter.ToString()) ||
                LPD.Digits.IsMatch(_currentCharacter.ToString()))
            {
                identifierSB.Append(_currentCharacter);
                _currentCharacter = GetNextChar();
            }

            var identifier = identifierSB.ToString();

            if (LPD.Keywords.ContainsKey(identifier))
            {
                Console.WriteLine("Keyword: {0}", identifier);
                return new Token(LPD.Keywords[identifier], identifier);
            }
            else
            {
                Console.WriteLine("Identifier: {0}", identifier);
                return new Token(LPD.Symbol.IDENTIFIER, identifier);
            }
        }

        private Token HandleNumber()
        {
            var number = new StringBuilder();

            number.Append(_currentCharacter);
            _currentCharacter = GetNextChar();

            while (char.IsDigit(_currentCharacter))
            {
                number.Append(_currentCharacter);
                _currentCharacter = GetNextChar();
            }

            Console.WriteLine("Number: {0}", number.ToString());
            return new Token(LPD.Symbol.NUMBER, number.ToString());
        }
    }

    /// <summary>
    /// For characters that are not supported in the current compiler.
    /// </summary>
    class NotSupportedCharacterException : Exception {
        public NotSupportedCharacterException() : base() { }
        public NotSupportedCharacterException(string message) : base(message) { }
        public NotSupportedCharacterException(string message, System.Exception inner) : base(message, inner) { }
    }
}
