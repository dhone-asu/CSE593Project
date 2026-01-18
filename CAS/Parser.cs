using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Parser
    {
        Tokenizer tokenizer;

        public Parser(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer; 
        }

        public void SyntaxError()
        {
            throw new ParserException("Syntax error");
        }

        public Token Expect(TokenType type)
        {
            Token t = tokenizer.GetToken();

            if (t.Type != type)
                SyntaxError();
  
            return t;
        }
        
        public void ParseInput()
        {
            ParseLine();
            Expect(TokenType.EndOfInput);
        }
        public void ParseLine()
        {
            Token token1 = tokenizer.Peek(1), token2 = tokenizer.Peek(2);

            if ((token1.Type == TokenType.Identifier) && (token2.Type == TokenType.Assign))
            {
                ParseAssignment();
            }
            else
            {
                ParseExpr();
            }
        }

        public void ParseAssignment()
        {
            Token id = Expect(TokenType.Identifier);

            Expect(TokenType.Assign);

            ParseExpr();

        }

        public void ParseExpr()
        {
            Token t = tokenizer.Peek(1);

            if (t.Type == TokenType.Lpar)
            {
                Expect(TokenType.Lpar);
                ParseExpr();
                Expect(TokenType.Rpar);
            }
            else if ((t.Type == TokenType.Identifier) || (t.Type == TokenType.Numeral))
            {
                ParseToken();
            }
            else
            {
                
            }
        }

        public void ParseToken()
        {
            Token t = tokenizer.Peek(1);

            if (t.Type == TokenType.Numeral)
                Expect(TokenType.Numeral);
            else if (t.Type == TokenType.Identifier)
                Expect(TokenType.Identifier);
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }
        public ParserException() : base() { }
    }
}
