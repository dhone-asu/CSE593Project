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
            string last = "";
            while (true)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    Console.WriteLine(last);
                    line = last;
                }
                last = line;
                //Evaluator evaluator = new Evaluator(line);
                //var result = evaluator.Evaluate();
                StringBuffer sb = new StringBuffer(line);
                var tok = new Tokenizer(sb);
                var parser = new Parser(tok);
                var result = parser.ParseInput();
                if (result is OpNode && ((OpNode)result).Operator == Operator.Mult)
                {
                    var left = Simplification.Polynomialize((result as OpNode).Left);
                    var right = Simplification.Polynomialize((result as OpNode).Right);
                    var product = left * right;
                    var expression = product.ToExpression();

                    Console.WriteLine(Evaluator.PrintExpression(expression));

                }
                SessionManager.Default.UserVariableManager.SetVariable("ans", new UserVariable
                {
                    Name = "ans",
                    Value = result
                });
                /*if (result is AtomNode)
                    Console.WriteLine(((AtomNode)result).Data);
                else
                    Console.WriteLine(Evaluator.PrintExpression(result));*/
                Console.WriteLine();
            }
        }
        //public enum Operator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte, Neg, Assign }

    }
}
