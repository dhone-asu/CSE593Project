using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public enum ParserContext { Assignment, Standalone, Params, Parentheses }
    public class Parser
    {
        private Tokenizer tokenizer;
        //public enum BinaryOperator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte }
        public static readonly Dictionary<Operator, int> PrecedenceMap = new Dictionary<Operator, int>
        {
            { Operator.None, -1 },
            { Operator.Pow, 0 },
            { Operator.Neg, 1 },
            { Operator.Mult, 2},
            { Operator.Div, 2},
            { Operator.Plus, 3 },
            { Operator.Minus, 3 },
            { Operator.Eq, 4 },
            { Operator.Neq, 4 },
            { Operator.Gt, 4 },
            { Operator.Lt, 4 },
            { Operator.Gte, 4 },
            { Operator.Lte, 4 },
        };

        public Parser(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer; 
        }

        public void SyntaxError(String message)
        {
            throw new ParserException($"Syntax error: {message}");
        }

        public Token Expect(TokenType type)
        {
            Token t = tokenizer.GetToken();

            if (t.Type != type)
                SyntaxError($"'{type}' expected");
  
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
                ExprNode node = ParseExpr(Operator.None, false);
                return;
            }
        }

        public void ParseAssignment()
        {
            Token id = Expect(TokenType.Identifier);

            Expect(TokenType.Assign);

            ExprNode node = ParseExpr(Operator.None, false);

            return;

        }

        public ExprNode ParseExpr(Operator prevOp, bool inParen)
        {
            Token t1 = tokenizer.Peek(1);
            Token t2 = tokenizer.Peek(2);
            ExprNode node1;
            ExprNode node2;
            Operator op;

            if (t1.Type == TokenType.LParen)
            {
                Expect(TokenType.LParen);
                node1 = ParseExpr(Operator.None, true);
                Expect(TokenType.RParen);
                return node1;
            }
            else if (t1.Type == TokenType.Minus)
            {
                Expect(TokenType.Minus);
                node1 = ParseExpr(Operator.Minus, inParen);
                OpNode opNode = new OpNode
                {
                    Operator = Operator.Neg,
                    Left = node1,
                };
                return opNode;
            }
            else if ((t1.Type == TokenType.Identifier) || (t1.Type == TokenType.Numeral))
            {
                if (t1.Type == TokenType.Identifier)
                    t1 = Expect(TokenType.Identifier);
                else
                    t1 = Expect(TokenType.Numeral);

                node1 = new AtomNode
                {
                    Name = t1.String,
                    Type = (t1.Type == TokenType.Identifier) ? AtomType.Identifier : AtomType.Numeral
                };

                if (t2.Type == TokenType.EndOfInput)
                    return node1;
                else if (inParen && t2.Type == TokenType.RParen)
                    return node1;

                op = ParseOp();

                if (prevOp == Operator.None || PrecedenceMap[op] < PrecedenceMap[prevOp])
                {
                    node2 = ParseExpr(op, inParen);

                    OpNode opNode = new OpNode
                    {
                        Operator = op,
                        Left = node1,
                        Right = node2
                    };

                    return opNode;
                }
                else
                {
                    return node1;
                }
            }
            else
            {
                SyntaxError($"Type '{t1.Type}' not expected");
                return null;
            }

        }

        public Operator ParseOp()
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
                SyntaxError($"Unexpected operator");
                return Operator.None; // This is to silence and error that op remains unassigned
            }

            return RuleManager.OperatorMap[op.Type];
        }

        public AtomNode ParseAtom()
        {
            Token t = tokenizer.Peek(1);

            if (t.Type == TokenType.Numeral)
            {
                t = Expect(TokenType.Numeral);
                return new AtomNode { Name = t.String, Type = AtomType.Numeral };
            }
            else if (t.Type == TokenType.Identifier)
            {
                t = Expect(TokenType.Identifier);
                return new AtomNode { Name = t.String, Type = AtomType.Identifier };
            }
            else
            {
                SyntaxError($"Unexpected atom type");
                return null;
            }
        }
    }

    public class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }
        public ParserException() : base() { }
    }
}
