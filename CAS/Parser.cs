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
        private Tokenizer tokenizer;
        //public enum BinaryOperator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte }
        public static readonly Dictionary<BinaryOperator, int> PrecedenceMap = new Dictionary<BinaryOperator, int>
        {
            { BinaryOperator.Pow, 1 },
            { BinaryOperator.Mult, 2},
            { BinaryOperator.Div, 2},
            { BinaryOperator.Plus, 3 },
            { BinaryOperator.Minus, 3  },
            { BinaryOperator.Eq, 4 },
            { BinaryOperator.Neq, 4 },
            { BinaryOperator.Gt, 4 },
            { BinaryOperator.Lt, 4 },
            { BinaryOperator.Gte, 4 },
            { BinaryOperator.Lte, 4 },
        };

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
                ExprNode node = ParseExpr();
                return;
            }
        }

        public void ParseAssignment()
        {
            Token id = Expect(TokenType.Identifier);

            Expect(TokenType.Assign);

            ExprNode node = ParseExpr();

            return;

        }

        public ExprNode ParseExpr()
        {
            Token t1 = tokenizer.Peek(1);
            Token t2 = tokenizer.Peek(2);
            ExprNode node1;
            ExprNode node2;
            BinaryOperator op;

            if (t1.Type == TokenType.Lpar)
            {
                Expect(TokenType.Lpar);
                node1 = ParseExpr();
                Expect(TokenType.Rpar);
                return node1;
            }
            else if ((t1.Type == TokenType.Identifier) || (t1.Type == TokenType.Numeral))
            {
                if (ExprNode.ExprOperators.HasFlag(t2.Type))
                {
                    node1 = ParseExpr();
                    op = ParseOp();
                    node2 = ParseExpr();
                    return new BinaryOpNode { Operator = op, Left  = node1, Right = node2 };
                }
                else
                {
                    return ParseTerm();

                }
            }
            else
            {
                SyntaxError();
                return null;
            }

        }

        public BinaryOperator ParseOp()
        {
            Token t = tokenizer.Peek(1);
            Token op;

            if (t.Type == TokenType.Plus)
                op = Expect(TokenType.Plus);
            else if (t.Type == TokenType.Minus)
                op = Expect(TokenType.Minus);
            else if (t.Type == TokenType.Div)
                op = Expect(TokenType.Div);
            else if (t.Type == TokenType.Mult)
                op = Expect(TokenType.Mult);
            else if (t.Type == TokenType.Pow)
                op = Expect(TokenType.Pow);
            else if (t.Type == TokenType.Mod)
                op = Expect(TokenType.Mod);
            else if (t.Type == TokenType.Eq)
                op = Expect(TokenType.Eq);
            else if (t.Type == TokenType.Neq)
                op = Expect(TokenType.Neq);
            else if (t.Type == TokenType.Gt)
                op = Expect(TokenType.Gt);
            else if (t.Type == TokenType.Lt)
                op = Expect(TokenType.Lt);
            else if (t.Type == TokenType.Gte)
                op = Expect(TokenType.Gte);
            else if (t.Type == TokenType.Lte)
                op = Expect(TokenType.Lte);
            else
            { 
                SyntaxError();
                return BinaryOperator.None; // This is to silence and error that op remains unassigned
            }

            return ExprNode.OperatorMap[op.Type];
        }

        public TermNode ParseTerm()
        {
            Token t = tokenizer.Peek(1);

            if (t.Type == TokenType.Numeral)
            {
                t = Expect(TokenType.Numeral);
                return new TermNode { Name = t.String, Type = TermType.Numeral };
            }
            else if (t.Type == TokenType.Identifier)
            {
                t = Expect(TokenType.Identifier);
                return new TermNode { Name = t.String, Type = TermType.Identifier };
            }
            else
            {
                SyntaxError();
                return null;
            }
        }
    }

    public enum ParserContext { Assignment, Standalone, Params, Parentheses }

    public class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }
        public ParserException() : base() { }
    }
}
