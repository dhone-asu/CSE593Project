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
    }
}
