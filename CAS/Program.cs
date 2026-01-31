using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using CAS.SymbolManagement;

namespace CAS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string line = Console.ReadLine();
                StringBuffer b = new StringBuffer(line);
                Tokenizer tokenizer = new Tokenizer(b);
                Parser parser = new Parser(tokenizer);
                ExprNode tree = parser.ParseInput();
                var result = Eval(tree);
                if (result is AtomNode)
                    Console.WriteLine(((AtomNode)result).Data);
                else
                    Console.WriteLine(PrintExpression(result));
                Console.WriteLine();
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

        static string PrintExpression(ExprNode tree)
        {
            if (tree == null) return "";
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;
                int treePrec = Parser.PrecedenceMap[opNode.Operator];
                int leftPrec = 0, rightPrec = 0;
                string leftExpr = null;
                string rightExpr = null;

                if (leftNode is OpNode)
                {
                    leftPrec = Parser.PrecedenceMap[((OpNode)leftNode).Operator];

                    leftExpr = PrintExpression(leftNode);

                    if (leftPrec > 0 && leftPrec < treePrec)
                        leftExpr = $"({leftExpr})";
                }
                else
                    leftExpr = PrintExpression(leftNode);
                if (rightNode is OpNode)
                {
                    rightPrec = Parser.PrecedenceMap[((OpNode)rightNode).Operator];
                    rightExpr = PrintExpression(rightNode);

                    if (rightPrec > 0 && rightPrec < treePrec)
                        rightExpr = $"({rightExpr})";
                }
                else 
                    rightExpr = PrintExpression(rightNode);



                if (rightExpr != null)
                {
                    string s = $"{leftExpr} {operatorToStringMap[opNode.Operator]} {rightExpr}";
                    return s;
                }
                else
                {
                    string s = $"{operatorToStringMap[opNode.Operator]} {rightExpr}";
                    return s;
                }
            }
            else if (tree is AtomNode)
            {
                var atomNode = (AtomNode)tree;
                string s = atomNode.Data.ToString();
                return s;
            }
            else if (tree is FunctionNode)
            {
                var funcNode = (FunctionNode)tree;
                string parameters = "";

                if (funcNode.Params.Any())
                {
                    parameters = $"{PrintExpression(funcNode.Params[0])}";
                    for (int i = 1; i < funcNode.Params.Count; i++)
                    {
                        string param = PrintExpression(funcNode.Params[i]);
                        parameters = $"{parameters}, {param}";
                    }
                }
                string s = $"{funcNode.Name}({parameters})";
                return s;
            }
            else
                throw new Exception("Node type not supported.");
        }

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
        static ConstantManager constantManager = new ConstantManager();

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
                AtomNode node = (AtomNode)tree;
                
                if (node.Type == AtomType.Identifier)
                {
                    if (constantManager.VariableExists((string)node.Data))
                    {
                        return new AtomNode
                        {
                            Type = AtomType.Numeral,
                            Data = ((AtomNode)constantManager.GetVariable((string)node.Data).Value).Data
                        };
                    }
                    else
                        return tree;
                }
                else
                    return tree;
            }
            else if (tree is FunctionNode)
            {
                FunctionNode functionNode = (FunctionNode)tree;
                object[] parameters = functionNode.Params.ToArray();
                for (int i = 0; i < parameters.Count(); i++)
                {
                    parameters[i] = Eval((ExprNode)parameters[i]);
                }

                if (nativeFunctionManager.FunctionExists(functionNode.Name))
                {

                    IFunction func = nativeFunctionManager.GetFunction(((FunctionNode)tree).Name);
                    object result = func.Call(parameters);

                    return (ExprNode)result;
                }
                else
                    return new FunctionNode
                    {
                        Name = functionNode.Name,
                        Params = new List<ExprNode>((ExprNode[])parameters)
                    };
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
                case Operator.Mod:
                    return a % b;
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
