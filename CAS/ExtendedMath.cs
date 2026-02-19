using CAS.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class ExtendedMath
    {
        public const double EulerMascheroni = 0.57721566490153286060651209;
        public static double Frac(double a)
        {
            return a % 1.0;
        }
        public static double Int(double a)
        {
            return a - a % 1.0;
        }

        public static double Mod(double a, double b)
        {
            return a % b;
        }
        public static double Fact(double a)
        {
            if ((a < 0) || (a % 1.0 > 0))
                return double.NaN;
            else
            {
                double ans = 1.0;
                for (double k = 1.0; k <= a; k += 1.0)
                {
                    ans *= k;
                }
                return ans;
            }
        }

        public static double Asinh(double a)
        {
            return Math.Log(a+Math.Sqrt(1+a*a));
        }

        public static double Acosh(double a)
        {
            return Math.Log(a + Math.Sqrt(a * a - 1));
        }

        public static double Atanh(double a)
        {
            return Math.Log((1 + a) / (1 - a)) / 2.0;
        }

        private static double NextW(double w, double x)
        {
            return w - (w*Math.Exp(w) - x)/((1+w)*Math.Exp(w));
        }

        public static double LambertW(double x)
        {
            if (x < -Math.Exp(-1))
                return double.NaN;
            double w = 1.0;

            while (true)
            {
                double wPrev = w;
                w = NextW(w, x);
                if (Math.Abs(w - wPrev) < 1E-10)
                    return w;
            }
        }

        public static double Integrate(Func<double,double> function, double a, double b)
        {
            double n = 1e8;
            double delta = (b - a) / n;
            double result = 0;

            for (double x = a;  x <= b; x += delta)
            {
                result += function(x) * delta;
            }

            return result;
        }

        public static double Differentiate(Func<double, double> function, double a)
        {
            double delta = 1e-8;

            return (function(a + delta) - function(a - delta)) / (2 * delta);
        }


        public static ExprNode Integrate(IFunction function, AtomNode a, AtomNode b)
        {
            if (a.Type != AtomType.Numeral || b.Type != AtomType.Numeral)
                throw new Exception("Limits must be numeric");
            double lowerLim = (double)a.Data;
            double upperLim = (double)b.Data;
            double n = 1e7;
            double delta = (upperLim - lowerLim)/n;
            double result = 0;

            for (double x = lowerLim; x <= upperLim; x += delta)
            {
                AtomNode xNode = new AtomNode
                {
                    Type = AtomType.Numeral,
                    Data = x,
                };
                AtomNode yNode = (AtomNode)function.Call(new object[] { xNode });
                result += (double)yNode.Data * delta;
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
            double delta = 1e-8;
            AtomNode plusdelta = new AtomNode { Data = (double)a.Data + delta, Type = AtomType.Numeral };
            AtomNode minusdelta = new AtomNode { Data = (double)a.Data - delta, Type = AtomType.Numeral };

            AtomNode f1 = (AtomNode)function.Call(new[] { plusdelta });
            AtomNode f2 = (AtomNode)function.Call(new[] { minusdelta });
            double result = ((double)f1.Data - (double)f2.Data) / (2 * delta);
            return new AtomNode { Data = result, Type = AtomType.Numeral };
        }

        private static double LogGammaBaseFormula(double x)
        {
            double c = Math.Log(2 * Math.PI) / 2.0;

            double logGamma = c + (x - 0.5) * Math.Log(x) - x
                + 1.0 / x * (1.0 / 12.0 - 1 / (x * x) * (1.0 / 360.0 - 1.0 / (x * x) * (1.0 / 1260.0 - 1.0 / (x * x) * (1.0 / 1680.0-1.0/(x*x)*(1.0/1188.0)))));

            return logGamma;
        }

        public static double CorrectedLogGamma(double x)
        {
            double c = LogGammaBaseFormula(x + 6) - Math.Log(x + 5) - Math.Log(x + 4) - Math.Log(x + 3) - Math.Log(x + 2) - Math.Log(x + 1) - Math.Log(x);
            return c;
        }

    }
}
