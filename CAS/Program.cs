using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
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

            Console.WriteLine(Eval(tree));
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

        static double Eval(ExprNode tree)
        {
            if (tree == null) return Double.NaN;
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;

                if (rightNode != null)
                {
                    double leftVal = Eval(leftNode);
                    double rightVal = Eval(rightNode);
                    double val = Compute(opNode.Operator, leftVal, rightVal);

                    return val;
                }
                else
                {
                    double leftVal = Eval(leftNode);
                    double val = Compute(opNode.Operator, leftVal, 0.0);

                    return val;
                }
            }
            else if (tree is AtomNode)
            {
                var atomNode = (AtomNode)tree;
                var s = atomNode.Data;
                double val;

                if (atomNode.Type == AtomType.Numeral)
                {
                    if (double.TryParse(s, out val))
                        return val;
                    else
                        throw new Exception($"{s} is not a valid numeral");
                }
                else
                    throw new Exception($"Identifiers not supported for evaluation");
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
