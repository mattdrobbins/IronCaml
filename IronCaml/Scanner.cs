using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class Scanner
    {
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private string _source;
        private List<Token> _tokens = new List<Token>();

        private Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "let", TokenType.LET },
            { "in", TokenType.IN }
        };

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));

            return _tokens;
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '=':
                    AddToken(TokenType.EQUAL);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case '\n':
                    _line++;
                    break;
                case ' ':
                case '\t':
                case '\r':
                    break;
                case '"': String(); 
                    break;
                case '\'': Char(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        IronCaml.Error(_line, "Unexpected Charactor");
                    }
                    break;
            }
        }

        private bool IsAtEnd() => _current >= _source.Length;

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            TokenType? type = null;

            var text = _source.Substring(_start, _current - _start);
            if (_keywords.ContainsKey(text))
            {
                type = _keywords[text];
            }
            else
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type.Value);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || c == '-' || c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void Number()
        {
            var type = TokenType.INTEGER;

            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                type = TokenType.FLOATINGPOINT;
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            if (type == TokenType.INTEGER)
            {
                AddToken(type, long.Parse(_source.Substring(_start, _current - _start)));
            }
            else
            {
                AddToken(type, double.Parse(_source.Substring(_start, _current - _start)));
            }
        }

        private void Char()
        {
            Advance();

            if (Peek() != '\'')
            {
                IronCaml.Error(_line, "Unterminated Char");
                return;
            }

            Advance();

            AddToken(TokenType.CHAR, _source[_current - 2]);
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }
            if (IsAtEnd())
            {
                IronCaml.Error(_line, "Unterminated String");
                return;
            }

            Advance();

            var value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.STRING, value);
        }


        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;
            _current++;
            return true;
        }

        private void AddToken(TokenType type, object literal)
        {
            var text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }
    }
}
