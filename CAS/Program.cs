using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string line = Console.ReadLine();
            StringBuffer b = new StringBuffer(line);
            Tokenizer tokenizer = new Tokenizer(b);
            Parser parser = new Parser(tokenizer);
            ExprNode tree = parser.ParseInput();

            var result = Eval(tree);

            return;
        }
        //public enum Operator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte, Neg, Assign }
        static Dictionary<Operator, string> operatorToStringMap = new Dictionary<Operator, string>
        {
            { Operator.None, "" },
            { Operator.Plus, "+" },
            { Operator.Minus, "-" },
            { Operator.Div, "/" },
            { Operator.Mult, "*" },
            { Operator.Pow, "^" },
            { Operator.Mod, "%" },
            { Operator.Eq, "=="  },
            { Operator.Neq, "!=" },
            { Operator.Gt, ">" },
            { Operator.Lt, "<" },
            { Operator.Gte, ">=" },
            { Operator.Lte, "<=" },
            { Operator.Neg, "-" },
            { Operator.Assign, "=" }
        };

        static string ToPostfix(ExprNode tree)
        {
            if (tree == null) return "";
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;
                if (rightNode != null)
                {
                    string s = $"{ToPostfix(leftNode)} {ToPostfix(rightNode)} {operatorToStringMap[opNode.Operator]}";
                    return s;
                }
                else
                {
                    string s = $"{ToPostfix(leftNode)} {operatorToStringMap[opNode.Operator]}";
                    return s;
                }
            }
            else if (tree is AtomNode)
            {
                var atomNode = (AtomNode)tree;
                string s = atomNode.Data;
                return s;
            }
            else
            {
                throw new Exception("Only OpNode and AtomNode supported");
                //return null;
            }
        }

        static ExprNode Eval(ExprNode tree)
        {
            if (tree == null) return null;
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;

                if (rightNode != null)
                {
                    ExprNode leftResult = Eval(leftNode);
                    ExprNode rightResult = Eval(rightNode);
                    if ((leftResult is AtomNode && ((AtomNode)leftResult).Type == AtomType.Numeral) &&
                        (rightResult is AtomNode && ((AtomNode)rightResult).Type == AtomType.Numeral))
                    {
                        double leftVal = 0, rightVal = 0;

                        if (double.TryParse(((AtomNode)leftResult).Data, out leftVal) &&
                            double.TryParse(((AtomNode)rightResult).Data, out rightVal))
                        {
                            return new AtomNode
                            {
                                Type = AtomType.Numeral,
                                Data = Compute(opNode.Operator, leftVal, rightVal).ToString()
                            };
                        }
                        else
                            throw new Exception($"One of '{((AtomNode)leftResult).Data}' or '{((AtomNode)rightResult).Data}' is invalid numeral");
                    }
                    else
                        return new OpNode
                        {
                            Operator = opNode.Operator,
                            Left = leftResult,
                            Right = rightResult
                        };
                }
                else
                {
                    ExprNode leftResult = Eval(leftNode);

                    if (leftResult is AtomNode && ((AtomNode)leftResult).Type == AtomType.Numeral)
                    {
                        double leftVal = 0;

                        if (double.TryParse(((AtomNode)leftResult).Data, out leftVal))
                        {
                            return new AtomNode
                            {
                                Type = AtomType.Numeral,
                                Data = Compute(opNode.Operator, leftVal, 0).ToString()
                            };
                        }
                        else
                            return opNode;
                    }
                    else
                        return opNode;
                }
            }
            else if (tree is AtomNode)
            {
                return tree;
            }
            else
                throw new Exception($"Node type '{tree.GetType()}' not supported for evaluation");
        }

        static double Compute(Operator op, double a, double b)
        {
            switch (op)
            {
                case Operator.Plus:
                    return a + b;
                case Operator.Minus:
                    return a - b;
                case Operator.Mult:
                    return a * b;
                case Operator.Div:
                    return a / b;
                case Operator.Pow:
                    return Math.Pow(a, b);
                case Operator.Neg:
                    return -a;
                default:
                    throw new Exception($"Operator {op} not supported for evaluation");
            }
        }


    }
}
