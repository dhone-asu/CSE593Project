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
                return new Token { TokenType = TokenType.EndOfInput };
            else
            {
                return tokens[index++];
            }
        }

        public void UnGetToken(int num)
        {
            if (num <= 0)
                throw new Exception("Argument must be greater than 0");
            index -= num;
            if (index < 0)
                throw new Exception("Cannot back up past first token");
        }

        public Token Peek(int num)
        {
            if (num <= 0)
                throw new Exception("Argument must be greater than 0");

            int tempIndex = index + num - 1;

            if (tempIndex >= tokens.Count)
                return new Token { TokenType = TokenType.EndOfInput };
            else
                return tokens[tempIndex];
        }
        

        private Token NextToken()
        {
            SkipSpace();
            char c;

            if (buffer.EndOfInput())
            {
                return new Token { String = "", TokenType = TokenType.EndOfInput };
            }
            else c = buffer.GetChar();

            switch (c)
            {
                case '+':
                    return new Token { TokenType = TokenType.Plus };
                case '-':
                    return new Token { TokenType = TokenType.Minus };
                case '/':
                    return new Token { TokenType = TokenType.Div };
                case '*':
                    if (buffer.PeekChar() == '*')
                    {
                        buffer.GetChar();
                        return new Token { TokenType = TokenType.Pow };
                    }
                    else
                    {
                        return new Token { TokenType = TokenType.Mult };
                    }
                case '%':
                    return new Token { TokenType = TokenType.Mod };
                case '=':
                    return new Token { TokenType = TokenType.Eq };
                case '<':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { TokenType = TokenType.Lte };
                    }
                    else
                    {
                        return new Token { TokenType = TokenType.Lt };
                    }
                case '>':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { TokenType = TokenType.Gte };
                    }
                    else
                    {
                        return new Token { TokenType = TokenType.Gt };
                    }
                case '!':
                    if (buffer.PeekChar() == '=')
                    {
                        buffer.GetChar();
                        return new Token { TokenType = TokenType.Neq };
                    }
                    else
                    {
                        return new Token { TokenType = TokenType.Error };
                    }
                case '(':
                    return new Token { TokenType = TokenType.Lpar };
                case ')':
                    return new Token { TokenType = TokenType.Rpar };
                case ',':
                    return new Token { TokenType = TokenType.Comma };
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
                        return new Token { TokenType = TokenType.Error };
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
                return new Token { String = str, TokenType = TokenType.Numeral };
            }

            c = buffer.PeekChar();

            while (char.IsDigit(c))
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            return new Token { String = str, TokenType = TokenType.Numeral };
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
                return new Token { TokenType = TokenType.Error };

            c = buffer.PeekChar();

            while (char.IsLetterOrDigit(c) || c=='_')
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            return new Token { String = str, TokenType = TokenType.Identifier };
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
        Error,
        EndOfInput }
    public class Token
    {
        public TokenType TokenType
        {
            get; set; 
        }
        public string String
        {
            get; set; 
        }

    }
}
