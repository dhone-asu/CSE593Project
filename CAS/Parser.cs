using System;
using System.Collections.Generic;

namespace CAS
{
    public enum ParserContext { General, Params }
    public class Parser
    {
        private Tokenizer tokenizer;
        //public enum BinaryOperator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte }
        public static readonly Dictionary<Operator, int> PrecedenceMap = new Dictionary<Operator, int>
        {
            { Operator.Pow, 5 },
            { Operator.Neg, 4 },
            { Operator.Mult, 3},
            { Operator.Div, 3},
            { Operator.Plus, 2 },
            { Operator.Minus, 2 },
            { Operator.Eq, 1 },
            { Operator.Neq, 1 },
            { Operator.Gt, 1 },
            { Operator.Lt, 1 },
            { Operator.Gte, 1 },
            { Operator.Lte, 1 },
        };

        public static readonly Dictionary<TokenType, Operator> OperatorMap = new Dictionary<TokenType, Operator>
        {
            {TokenType.Plus, Operator.Plus},
            {TokenType.Minus, Operator.Minus},
            {TokenType.Div, Operator.Div},
            {TokenType.Mult, Operator.Mult},
            {TokenType.Pow, Operator.Pow},
            {TokenType.Mod, Operator.Mod},
            {TokenType.Eq, Operator.Eq},
            {TokenType.Gt, Operator.Gt},
            {TokenType.Lt, Operator.Lt},
            {TokenType.Gte, Operator.Gte},
            {TokenType.Lte, Operator.Lte},
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
        
        public ExprNode ParseInput()
        {
            ExprNode tree = ParseLine();
            Expect(TokenType.EndOfInput);
            return tree;
        }
        public ExprNode ParseLine()
        {
            Token token1 = tokenizer.Peek(1), token2 = tokenizer.Peek(2);
            ExprNode tree = null;

            if ((token1.Type == TokenType.Identifier) && (token2.Type == TokenType.Assign))
            {
                tree = ParseAssignment();
            }
            else
            {
                tree = ParseExpr(1, false, ParserContext.General);
            }

            return tree;
        }

        public ExprNode ParseAssignment()
        {
            Token id = Expect(TokenType.Identifier);

            Expect(TokenType.Assign);

            ExprNode node = ParseExpr(1, false, ParserContext.General);

            OpNode tree = new OpNode
            {
                Operator = Operator.Assign,
                Left = new AtomNode
                {
                    Data = id.String,
                    Type = AtomType.Identifier
                },
                Right = node
            };
             
            return tree;

        }

        // Operator precedence parsing adapted from Eli Bendersky's website:
        //  https://eli.thegreenplace.net/2012/08/02/parsing-expressions-by-precedence-climbing
        public ExprNode ParseExpr(int minPrec, bool inParen, ParserContext context)
        {
            ExprNode lhs = ParseOperand(inParen, context);
            ExprNode rhs = null;
            Token cur = null;

            while (true)
            {
                cur = tokenizer.Peek(1);

                if ((cur.Type == TokenType.EndOfInput) || (inParen && cur.Type == TokenType.RParen))
                    break;
                else if (!inParen && context == ParserContext.Params && (cur.Type == TokenType.Comma || cur.Type == TokenType.RParen))
                    break;

                Operator op = ParseOperator();

                int prec = PrecedenceMap[op];

                if (prec < minPrec)
                {
                    tokenizer.UnGetToken(1);
                    break;
                }

                int nextMinPrec = (op == Operator.Pow) ? prec : prec + 1; // Exponentiation is right-associative

                rhs = ParseExpr(nextMinPrec, inParen, context);

                lhs = BuildOpNode(op, lhs, rhs);
            }

            return lhs;
        }

        public ExprNode BuildOpNode(Operator op, ExprNode lhs, ExprNode rhs)
        {
            return new OpNode
            {
                Operator = op,
                Left = lhs,
                Right = rhs,
            };
        }

        public Operator ParseOperator()
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
                SyntaxError($"Unexpected operator '{t.Type}'");
                return Operator.None; // This is to silence and error that op remains unassigned
            }

            return OperatorMap[op.Type];
        }

        public ExprNode ParseOperand(bool inParen, ParserContext context)
        {
            Token t = tokenizer.Peek(1);

            if (t.Type == TokenType.Numeral)
            {
                t = Expect(TokenType.Numeral);
                return new AtomNode { Data = t.String, Type = AtomType.Numeral };
            }
            else if (t.Type == TokenType.Identifier)
            {
                t = Expect(TokenType.Identifier);
                Token t2 = tokenizer.Peek(1);

                if (t2.Type == TokenType.LParen)
                {
                    Expect(TokenType.LParen);
                    Token t3 = tokenizer.Peek(1);

                    if (t3.Type == TokenType.RParen)
                    {
                        Expect(TokenType.RParen);
                        return new FunctionNode
                        {
                            Name = t.String,
                            Params = new List<ExprNode>()
                        };
                    }
                    else
                    {
                        ExprNode function = ParseParams(t.String);
                        Expect(TokenType.RParen);
                        return function;
                    }
                }
                else
                {
                    return new AtomNode { Data = t.String, Type = AtomType.Identifier };
                }
            }
            else if (t.Type == TokenType.Minus)
            {
                Expect(TokenType.Minus);
                ExprNode node = ParseExpr(PrecedenceMap[Operator.Neg], inParen, context);
                return new OpNode
                {
                    Operator = Operator.Neg,
                    Left = node
                };
            }
            else if (t.Type == TokenType.LParen)
            {
                Expect(TokenType.LParen);
                ExprNode node = ParseExpr(1, true, context);
                Expect(TokenType.RParen);
                return node;
            }
            else
            {
                SyntaxError($"Unexpected atom type '{t.Type}'");
                return null;
            }
        }

        public FunctionNode ParseParams(string name)
        {
            ExprNode node1 = ParseExpr(1, false, ParserContext.Params);

            Token t1 = tokenizer.Peek(1);

            if (t1.Type == TokenType.RParen)
            {
                return new FunctionNode
                {
                    Name = name,
                    Params = new List<ExprNode> { node1 }
                };
            }
            else if (t1.Type == TokenType.Comma)
            {
                Expect(TokenType.Comma);
                FunctionNode functionNode = ParseParams(name);
                functionNode.Params.Insert(0, node1);
                return functionNode;
            }
            else
            {
                SyntaxError($"Unexpected token '{t1.Type}' in parameter list for '{name}'");
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
