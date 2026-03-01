using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class Expansion
    {

        public static ExprNode Expand(ExprNode node)
        {
            var simplified = Simplification.Simplify(node);
            var poly = Simplification.Polynomialize(simplified);
            return null;
        }
    }
}
