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
                return HandleIdentifierAndReservedWord();

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
            Token result = new Token(0, null);

            switch (_currentCharacter)
            {
                case ';':
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result =  new Token((int)Token.LPDSymbol.DOT, _currentCharacter.ToString());
                    break;

                case '.': 
                    Console.WriteLine("Punctuation: {0}", _currentCharacter);
                    result = new Token((int)Token.LPDSymbol.DOT, _currentCharacter.ToString());
                    break;
            }

            _currentCharacter = GetNextChar();
            return result;
        }

        private Token HandleRelationalOperators()
        {
            throw new NotImplementedException();
        }

        private Token HandleArithmeticOperator()
        {
            throw new NotImplementedException();
        }

        private Token HandleAttribution()
        {
            throw new NotImplementedException();
        }

        private Token HandleIdentifierAndReservedWord()
        {
            var identifier = new StringBuilder();

            identifier.Append(_currentCharacter);
            _currentCharacter = GetNextChar();

            while (char.IsLetter(_currentCharacter))
            {
                identifier.Append(_currentCharacter);
                _currentCharacter = GetNextChar();
            }

            Console.WriteLine("Identifier: {0}", identifier.ToString());
            return new Token((int)Token.LPDSymbol.IDENTIFIER, identifier.ToString());
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
