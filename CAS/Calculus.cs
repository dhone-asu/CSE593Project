using CAS.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Calculus
    {
        private const double DerivativeDelta = 1e-8;
        private const double IntegralN = 1e6;
        private const double IntegralInf = 1e4;

        public static ExprNode Derivative(ExprNode expression, string variable)
        {
            var poly = Simplification.Polynomialize(expression);

            return poly.Derivative(variable).ToExpression();
        }

        public static ExprNode Derivative(ExprNode expression, string variable, uint order)
        {
            if (order == 0)
                return expression;

            var poly = Simplification.Polynomialize(expression);

            for (int i = 0; i < order; i++)
                poly = poly.Derivative(variable);

            return poly.ToExpression();
        }

        public static double Integrate(Func<double, double> function, double a, double b)
        {
            double n = IntegralN;
            double delta = (b - a) / n;
            double result = 0;

            for (double x = a; x <= b; x += delta)
            {
                result += function(x) * delta;
            }

            return result;
        }

        public static double Differentiate(Func<double, double> function, double a)
        {
            double delta = DerivativeDelta;

            return (function(a + delta) - function(a)) / delta;
        }

        private static double CallFunction(IFunction function, double a)
        {
            AtomNode aNode = new AtomNode
            {
                Type = AtomType.Numeral,
                Data = a
            };
            AtomNode resultNode = (AtomNode)function.Call(new object[] { aNode });
            return (double)resultNode.Data;
        }

        public static ExprNode Integrate(IFunction function, AtomNode a, AtomNode b)
        {
            if (a.Type != AtomType.Numeral || b.Type != AtomType.Numeral)
                throw new Exception("Limits must be numeric");
            double lowerLim = (double)a.Data;
            double upperLim = (double)b.Data;

            if (double.IsInfinity(upperLim))
                upperLim = Math.Sign(upperLim) * IntegralInf;
            if (double.IsInfinity(lowerLim))
                lowerLim = Math.Sign(lowerLim) * IntegralInf;

            double n = IntegralN;
            double delta = (upperLim - lowerLim) / n;
            double result = 0;

            for (double x = lowerLim; x < upperLim; x += delta)
            {
                double h = delta / 2;
                if (x + 2 * h > upperLim)
                {
                    h = (upperLim - x) / 2;
                }

                double f1 = CallFunction(function, x);
                double f2 = CallFunction(function, x + h);
                double f3 = CallFunction(function, x + 2 * h);
                result += h / 3 * (f1 + 4 * f2 + f3);
            }

            return new AtomNode
            {
                Type = AtomType.Numeral,
                Data = result
            };
        }

        public static ExprNode Differentiate(IFunction function, AtomNode a)
        {
            if (a.Type != AtomType.Numeral)
                throw new Exception("Point must be numeric");
            double delta = DerivativeDelta;
            AtomNode plusdelta = new AtomNode { Data = (double)a.Data + delta, Type = AtomType.Numeral };
            AtomNode atA = new AtomNode { Data = (double)a.Data, Type = AtomType.Numeral };

            AtomNode f1 = (AtomNode)function.Call(new[] { plusdelta });
            AtomNode f2 = (AtomNode)function.Call(new[] { atA });
            double result = ((double)f1.Data - (double)f2.Data) / delta;
            return new AtomNode { Data = result, Type = AtomType.Numeral };
        }
    }
}
