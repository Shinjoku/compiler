using Compiler.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.General
{
    class Lexical
    {
        private StreamReader _file;
        private PointerPosition _position;
        private List<Token> _tokens;
        private bool _reachedEndOfFile;
        private char _currentCharacter;

        #region Character Lists

        private readonly List<char> SpaceCharacters = new List<char>
        {
            ' ',
            '\t',
            '\n',
            '\r'
        };

        private readonly List<char> ArithmeticOperators = new List<char>
        {
            '+',
            '-',
            '*'
        };

        private readonly List<char> RelationalOperators = new List<char>
        {
            '<',
            '>',
            '=',
            '!'
        };

        private readonly List<char> PunctuationCharacters = new List<char>
        {
            ';',
            ',',
            '(',
            ')',
            '.'
        };

        #endregion

        public Lexical()
        {
            _tokens = new List<Token>();
        }

        private void OpenFile(string filePath)
        {
            _file = new StreamReader(filePath, Encoding.UTF8);
            _position = new PointerPosition();
        }

        public char GetNextChar()
        {
            try
            {
                var result = (char)_file.Read();
                _position.Column++;

                if (result == '\n')
                {
                    _position.Column = 1;
                    _position.Line++;
                }

                if(result == ushort.MaxValue) _reachedEndOfFile = true;
                return result;
            }
            catch (IOException)
            {
                _reachedEndOfFile = true;
                return (char)0;
            }
        }

        private void PassComment()
        {
            while (_currentCharacter != '}' && !_reachedEndOfFile)
            {
                _currentCharacter = GetNextChar();
            }

            // Current char is '}', so let's get another _currentCharacter
            _currentCharacter = GetNextChar();
        }

        private void PassSpace()
        {
            while (SpaceCharacters.Contains(_currentCharacter) && !_reachedEndOfFile)
            {
                _currentCharacter = GetNextChar();
            }
        }

        private void CloseFile()
        {
            _file.Close();
        }

        public async Task<bool> Run(string filePath)
        {
            try
            {
                OpenFile(filePath);
                _currentCharacter = GetNextChar();

                while (!_reachedEndOfFile)
                {
                    while((_currentCharacter == '{' || SpaceCharacters.Contains(_currentCharacter)) &&
                        !_reachedEndOfFile)
                    {
                        if (_currentCharacter == '{')
                            PassComment();

                        PassSpace();
                    }

                    if (!_reachedEndOfFile)
                        _tokens.Add(GetToken());
                }

                CloseFile();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Token GetToken()
        {
            if (char.IsDigit(_currentCharacter))
                return HandleNumber();

            else if (char.IsLetter(_currentCharacter))
                return HandleIdentifierAndKeyWord();

            else if (_currentCharacter == ':')
                return HandleAttribution();

            else if (ArithmeticOperators.Contains(_currentCharacter))
                return HandleArithmeticOperator();

            else if (RelationalOperators.Contains(_currentCharacter))
                return HandleRelationalOperators();

            else if (PunctuationCharacters.Contains(_currentCharacter))
                return HandlePunctuation();

            else throw new NotSupportedCharacterException();
        }

        private Token HandlePunctuation()
        {
            Token result;
            string symbol = _currentCharacter.ToString();

            switch (_currentCharacter)
            {
                case ';':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result =  new Token((int)Token.LPDSymbol.DOT, symbol);
                    break;

                case '.': 
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token((int)Token.LPDSymbol.DOT, symbol);
                    break;

                case '(':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token((int)Token.LPDSymbol.OPEN_PARENTHESIS, symbol);
                    break;

                case ')':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token((int)Token.LPDSymbol.CLOSE_PARENTHESIS, symbol);
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
                        result = new Token((int)Token.LPDSymbol.GREATER_EQUAL, op);
                    }
                    else
                    {
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token((int)Token.LPDSymbol.GREATER, op);
                    }
                    break;

                case "<":
                    _currentCharacter = GetNextChar();
                    if (_currentCharacter == '=')
                    {
                        op += _currentCharacter;
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token((int)Token.LPDSymbol.LESSER_EQUAL, op);
                    }
                    else
                    {
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token((int)Token.LPDSymbol.LESSER, op);
                    }
                    break;

                case "=":
                    Console.WriteLine("Relational Operator: {0}", op);
                    result = new Token((int)Token.LPDSymbol.EQUAL, op);
                    break;

                case "!":
                    _currentCharacter = GetNextChar();
                    if (_currentCharacter == '=')
                    {
                        op += _currentCharacter;
                        Console.WriteLine("Relational Operator: {0}", op);
                        result = new Token((int)Token.LPDSymbol.DIFFERENT, op);
                    }
                    else throw new Exception();
                    break;

                default:
                    Console.WriteLine("RELATIONAL TOKEN CANNOT BE FOUND!");
                    return null;
            }

            _currentCharacter = GetNextChar();
            return result;
        }

        private Token HandleArithmeticOperator()
        {
            string op = _currentCharacter.ToString();
            Token result;

            switch (_currentCharacter)
            {
                case '+':
                    _currentCharacter = GetNextChar();
                    Console.WriteLine("Arithmetic Operator: {0}", op);
                    result = new Token((int)Token.LPDSymbol.PLUS, op);
                    break;

                case '-':
                    _currentCharacter = GetNextChar();
                    Console.WriteLine("Arithmetic Operator: {0}", op);
                    result = new Token((int)Token.LPDSymbol.MINUS, op);
                    break;

                case '*':
                    _currentCharacter = GetNextChar();
                    Console.WriteLine("Arithmetic Operator: {0}", op);
                    result = new Token((int)Token.LPDSymbol.MULTIPLICATION, op);
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
                return new Token((int)Token.LPDSymbol.COLON, characters);
            }
            else
            {
                characters += _currentCharacter;
                Console.WriteLine("Attribution: {0}", characters);
                return new Token((int)Token.LPDSymbol.ATTRIBUTION, characters);
            }
        }

        private Token HandleIdentifierAndKeyWord()
        {
            var identifierSB = new StringBuilder();

            identifierSB.Append(_currentCharacter);
            _currentCharacter = GetNextChar();

            while (char.IsLetter(_currentCharacter))
            {
                identifierSB.Append(_currentCharacter);
                _currentCharacter = GetNextChar();
            }

            var identifier = identifierSB.ToString();

            if (Token.Keywords.ContainsKey(identifier))
            {
                Console.WriteLine("Keyword: {0}", identifier);
                return new Token((int)Token.Keywords[identifier], identifier);
            }
            else
            {
                Console.WriteLine("Identifier: {0}", identifier);
                return new Token((int)Token.LPDSymbol.IDENTIFIER, identifier);
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
            return new Token((int)Token.LPDSymbol.NUMBER, number.ToString());
        }
    }

    /// <summary>
    /// For characters that are not supported in the current compiler.
    /// </summary>
    class NotSupportedCharacterException : Exception { }
}
