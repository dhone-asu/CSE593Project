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
                return new Token { Type = TokenType.EndOfInput };
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
                return new Token { Type = TokenType.EndOfInput };
            else
                return tokens[tempIndex];
        }
        

        private Token NextToken()
        {
            SkipSpace();
            char c;

            if (buffer.EndOfInput())
            {
                return new Token { String = "", Type = TokenType.EndOfInput };
            }
            else c = buffer.GetChar();

            switch (c)
            {
                case '+':
                    return new Token { Type = TokenType.Plus };
                case '-':
                    return new Token { Type = TokenType.Minus };
                case '/':
                    return new Token { Type = TokenType.Div };
                case '*':
                    if (buffer.PeekChar() == '*')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Pow };
                    }
                    else
                    {
                        return new Token { Type = TokenType.Mult };
                    }
                case '%':
                    return new Token { Type = TokenType.Mod };
                case '=':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Eq };
                    }
                    else
                        return new Token { Type = TokenType.Assign };
                case '<':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Lte };
                    }
                    else
                    {
                        return new Token { Type = TokenType.Lt };
                    }
                case '>':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Gte };
                    }
                    else
                    {
                        return new Token { Type = TokenType.Gt };
                    }
                case '!':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { Type = TokenType.Neq };
                    }
                    else
                    {
                        return new Token { Type = TokenType.Error };
                    }
                case '(':
                    return new Token { Type = TokenType.Lpar };
                case ')':
                    return new Token { Type = TokenType.Rpar };
                case ',':
                    return new Token { Type = TokenType.Comma };
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
                        return new Token { Type = TokenType.Error };
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
            else
            {
                return new Token { String = str, Type = TokenType.Numeral };
            }

            c = buffer.PeekChar();

            while (char.IsDigit(c))
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
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
    public enum TokenType { Numeral, Identifier,
        Plus, Minus, Div, Mult, Pow, Mod,
        Eq, Neq, Gt, Lt, Gte, Lte,
        Lpar, Rpar, Comma,
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
