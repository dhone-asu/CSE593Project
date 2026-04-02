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

                var result = Parser.Parse(line);

   
                var evaluated = Simplification.Resolve(result);
                Console.WriteLine(Evaluator.PrintExpression(evaluated));

                SessionManager.Default.UserVariableManager.SetVariable("ans", new UserVariable
                {
                    Name = "ans",
                    Value = evaluated
                });

                Console.WriteLine();
            }
        }
        //public enum Operator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte, Neg, Assign }

    }
}
