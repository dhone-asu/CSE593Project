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
            SessionManager sessionManager = new SessionManager();
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
                Evaluator evaluator = new Evaluator(line) { SessionManager = sessionManager };
                var result = evaluator.Evaluate();
                Evaluator resultEvaluator = new Evaluator(result);
                sessionManager.UserVariableManager.SetVariable("ans", new UserVariable
                {
                    Name = "ans",
                    Value = result
                });
                if (result is AtomNode)
                    Console.WriteLine(((AtomNode)result).Data);
                else
                    Console.WriteLine(resultEvaluator.ToString());
                Console.WriteLine();
            }


            return;
        }
        //public enum Operator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte, Neg, Assign }

    }
}
