using System;
using System.Collections.Generic;
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
        public Tokenizer(IBuffer buffer)
        {
            this.buffer = buffer;

            while (!buffer.EndOfInput())
            {
                tokens.Add(Tokenize());
            }

            return;
        }

        private Token Tokenize()
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
                    if (IsAlpha(c))
                    {
                        buffer.UnGetChar(c);
                        return GetIdent();
                    }
                    else if (IsDigitOrDecimal(c))
                    {
                        buffer.UnGetChar(c);
                        return GetNumeral();
                    }
                    else
                    {
                        return new Token { TokenType = TokenType.Error };
                    }
                    return new Token { TokenType = TokenType.EndOfInput };
                    break;
            }
        }

        public Token GetNumeral()
        {
            string str = "";
            char c = buffer.PeekChar();

            while (IsDigit(c))
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

            while (IsDigit(c))
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            return new Token { String = str, TokenType = TokenType.Numeral };
        }

        public Token GetIdent()
        {
            string str = "";
            char c = buffer.GetChar();

            if (IsAlpha(c))
            {
                str = str + c;
            }
            else
                return new Token { TokenType = TokenType.Error };

            c = buffer.PeekChar();

            while (IsAlphaNumOrUnderscore(c))
            {
                buffer.GetChar();
                str = str + c;
                c = buffer.PeekChar();
            }

            return new Token { String = str, TokenType = TokenType.Identifier };
        }

        public void SkipSpace()
        {
            char c = buffer.PeekChar();

            while (!buffer.EndOfInput() && IsSpace(c))
            {
                buffer.GetChar();
                c = buffer.PeekChar();
            }
        }

        public bool IsSpace(char c)
        {
            return " \t".Contains(c);
        }

        public bool IsDigit(char c)
        {
            return "0123456789".Contains(c);
        }

        public bool IsAlphaNumOrUnderscore(char c)
        {
            return IsDigit(c) || IsAlpha(c) || c == '_';
        }

        public bool IsDigitOrDecimal(char c)
        {
            return IsDigit(c) || c == '.';
        }
        public bool IsAlpha(char c) 
        {
            return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c);
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
