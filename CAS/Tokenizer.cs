using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Tokenizer
    {
        public IBuffer buffer;
        public List<Token> tokens = new List<Token>();
        private int index = 0;
        public Tokenizer(IBuffer buffer)
        {
            this.buffer = buffer;

            while (!buffer.EndOfInput())
            {
                tokens.Add(NextToken());
            }
        }

        public Token GetToken()
        {
            if (index >= tokens.Count)
                return new Token { Type = TokenType.EndOfInput, String = "$" };
            else
            {
                return tokens[index++];
            }
        }

        public void UnGetToken(int num)
        {
            if (num <= 0)
                throw new TokenizerException("Argument must be greater than 0");
            index -= num;
            if (index < 0)
                throw new TokenizerException("Cannot back up past first token");
        }

        public Token Peek(int num)
        {
            if (num <= 0)
                throw new TokenizerException("Argument must be greater than 0");

            int tempIndex = index + num - 1;

            if (tempIndex >= tokens.Count)
                return new Token { Type = TokenType.EndOfInput, String = "$" };
            else
                return tokens[tempIndex];
        }
        

        private Token NextToken()
        {
            SkipSpace();
            char c;

            if (buffer.EndOfInput())
            {
                return new Token { Type = TokenType.EndOfInput, String = "$" };
            }
            else c = buffer.GetChar();

            switch (c)
            {
                case '+':
                    return new Token { Type = TokenType.Plus, String = "+" };
                case '-':
                    return new Token { Type = TokenType.Minus, String = "-" };
                case '/':
                    return new Token { Type = TokenType.Div, String = "/" };
                case '*':
                    if (buffer.PeekChar() == '*')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Pow, String="**" };
                    }
                    else
                    {
                        return new Token { Type = TokenType.Mult, String="*" };
                    }
                case '^':
                    return new Token { Type = TokenType.Pow, String = "^" };
                case '=':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Eq, String = "==" };
                    }
                    else
                        return new Token { Type = TokenType.Assign, String = "=" };
                case '(':
                    return new Token { Type = TokenType.LParen, String = "(" };
                case ')':
                    return new Token { Type = TokenType.RParen, String = ")" };
                case ',':
                    return new Token { Type = TokenType.Comma, String = "," };
                default:
                    if (char.IsLetter(c))
                    {
                        buffer.UnGetChar(c);
                        return GetIdent();
                    }
                    else if (char.IsDigit(c) || c=='.')
                    {
                        buffer.UnGetChar(c);
                        return GetNumeral();
                    }
                    else
                    {
                        return new Token { Type = TokenType.Error, String = $"{c}" };
                    }
            }
        }

        private Token GetNumeral()
        {
            string str = "";
            char c = buffer.PeekChar();

            while (char.IsDigit(c))
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            c = buffer.PeekChar();

            if (c == '.')
            {
                buffer.GetChar();
                str = str + c;
            }

            c = buffer.PeekChar();

            while (char.IsDigit(c))
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            if (str == ".")
                throw new ParserException($"'{str}' is not a valid numeral");

            c = buffer.PeekChar();
            
            if (c == 'e' || c == 'E')  // Scientific number
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();

                if (c == '-' || c == '+')
                {
                    buffer.GetChar();
                    str = str + c;
                    c = buffer.PeekChar();
                }

                if (!char.IsDigit(c))
                    throw new TokenizerException("Exponent must follow E in scientific number");

                while (char.IsDigit(c))
                {
                    buffer.GetChar();
                    str = str + c;
                    c = buffer.PeekChar();
                }
            }

            return new Token { String = str, Type = TokenType.Numeral };
        }

        private Token GetIdent()
        {
            string str = "";
            char c = buffer.GetChar();

            if (char.IsLetter(c))
            {
                str = str + c;
            }
            else
                return new Token { Type = TokenType.Error };

            c = buffer.PeekChar();

            while (char.IsLetterOrDigit(c) || c=='_')
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            return new Token { String = str, Type = TokenType.Identifier };
        }

        private void SkipSpace()
        {
            char c = buffer.PeekChar();

            while (!buffer.EndOfInput() && char.IsWhiteSpace(c))
            {
                buffer.GetChar();
                c = buffer.PeekChar();
            }
        }
    }

    public interface IBuffer
    {
        char GetChar();
        char PeekChar();
        void UnGetChar(char c);
        void UnGetString(string s);
        bool EndOfInput();
    }

    [Flags]
    public enum TokenType { Numeral, Identifier,
        Plus, Minus, Div, Mult, Pow,
        Eq, Neq, Gt, Lt, Gte, Lte,
        LParen, RParen, Comma,
        Assign,
        Error,
        EndOfInput }

    public class Token
    {
        public TokenType Type
        {
            get; set; 
        }
        public string String
        {
            get; set; 
        }

    }

    public class TokenizerException : Exception
    {
        public TokenizerException(string message) : base(message) { }
        public TokenizerException() : base() { }
    }
}
