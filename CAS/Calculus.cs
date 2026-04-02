using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Calculus
    {

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
    }
}
