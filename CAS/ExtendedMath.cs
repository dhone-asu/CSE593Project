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
        public static readonly double EulerMascheroni = 0.57721566490153286060651209;
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
