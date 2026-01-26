using System;
using System.Collections.Generic;
using System.Linq;
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

            //Console.WriteLine(Traverse(tree));
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

        static string Traverse(ExprNode tree)
        {
            if (tree == null) return "";
            if (tree is OpNode)
            {
                var opNode = (OpNode)tree;
                var leftNode = opNode.Left;
                var rightNode = opNode.Right;
                if (rightNode != null)
                {
                    string s = $"{Traverse(leftNode)} {Traverse(rightNode)} {operatorToStringMap[opNode.Operator]}";
                    return s;
                }
                else
                {
                    string s = $"{Traverse(leftNode)} {operatorToStringMap[opNode.Operator]}";
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
    }
}
