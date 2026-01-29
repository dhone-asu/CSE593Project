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
            //try
            {
                Tokenizer tokenizer = new Tokenizer(b);
                Parser parser = new Parser(tokenizer);
                ExprNode tree = parser.ParseInput();
                var result = Eval(tree);
                Console.WriteLine(((AtomNode)result).Data);
            }
            //catch (Exception e)
            {
            //    Console.WriteLine($"{e.GetType().Name} has been thrown with message \"{e.Message}\"");
            //    return;
            }


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
                string s = atomNode.Data.ToString();
                return s;
            }
            else
            {
                throw new Exception("Only OpNode and AtomNode supported");
                //return null;
            }
        }

        static NativeFunctionManager nativeFunctionManager = new NativeFunctionManager();

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
                        double leftVal = (double)((AtomNode)leftResult).Data, rightVal = (double)((AtomNode)rightResult).Data;
                        return new AtomNode
                        {
                            Type = AtomType.Numeral,
                            Data = Compute(opNode.Operator, leftVal, rightVal)
                        };
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
                        double leftVal = (double)((AtomNode)leftResult).Data;
                        return new AtomNode
                        {
                            Type = AtomType.Numeral,
                            Data = Compute(opNode.Operator, leftVal, 0)
                        };
                    }
                    else
                        return opNode;
                }
            }
            else if (tree is AtomNode)
            {
                return tree;
            }
            else if (tree is FunctionNode)
            {
                FunctionNode functionNode = (FunctionNode)tree;
                if (nativeFunctionManager.FunctionExists(functionNode.Name))
                {
                    object[] parameters = functionNode.Params.ToArray();
                    for (int i = 0; i < parameters.Count(); i++)
                    {
                        parameters[i] = Eval((ExprNode)parameters[i]);
                    }
                    Function func = nativeFunctionManager.GetFunction(((FunctionNode)tree).Name);
                    object result = func.Call(parameters);

                    return (ExprNode)result;
                }
                else
                    throw new Exception($"Cannot find function {functionNode.Name}");
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
