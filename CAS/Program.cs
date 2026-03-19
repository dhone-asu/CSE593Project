using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
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
                //var expanded = Expansion.Expand(result);
                var evaluated = Simplification.Resolve(result);
                Console.WriteLine(Evaluator.PrintExpression(evaluated));
                /*if (result is OpNode && ((OpNode)result).Operator == Operator.Pow)
                {
                    var left = Simplification.Polynomialize((result as OpNode).Left);
                    var right = Simplification.Simplify((result as OpNode).Right);
                    
                    if (right is AtomNode && (right as AtomNode).Type == AtomType.Numeral)
                    {
                        double pow = (double)(right as AtomNode).Data;

                        var expr = left ^ pow;

                        Console.WriteLine(Evaluator.PrintExpression(expr));
                    }
                    else
                    {
                        var rightPoly = Simplification.Polynomialize(right);
                        var expr = left ^ rightPoly;

                        Console.WriteLine(Evaluator.PrintExpression(expr));
                    }

                }
                else if (result is OpNode && (result as OpNode).Operator == Operator.Mult)
                {
                    var left = Simplification.Polynomialize((result as OpNode).Left);
                    var right = Simplification.Polynomialize((result as OpNode).Right);
                    var product = left * right;

                    Console.WriteLine(Evaluator.PrintExpression(product.ToExpression()));
                }
                else
                {
                    var expression = Simplification.Simplify(result);
                    Console.WriteLine(Evaluator.PrintExpression(expression));
                }*/
                SessionManager.Default.UserVariableManager.SetVariable("ans", new UserVariable
                {
                    Name = "ans",
                    Value = evaluated
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
