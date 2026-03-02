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
            if (node is AtomNode || node is FunctionNode)
                return node;

            var simplified = Simplification.Simplify(node);
            var poly = Simplification.Polynomialize(simplified);
            var newPoly = new Polynomial();
            
            foreach (var term in poly.Terms)
            {
                Polynomial expandedTerm = null;

                foreach (var factor in term.Factors)
                {
                    var polyBase = Simplification.Polynomialize(Expand(factor.Base));
                    var polyOrder = Simplification.Polynomialize(Expand(factor.Order));
                    polyBase = Simplification.SimplifyTerms(polyBase);
                    polyOrder = Simplification.SimplifyTerms(polyOrder);

                    var newExpr = polyBase ^ polyOrder;
                    var expPoly = Simplification.Polynomialize(newExpr);

                    if (expandedTerm == null)
                        expandedTerm = expPoly;
                    else
                        expandedTerm *= expPoly;
                }

                newPoly += expandedTerm;
            }

            return newPoly.ToExpression();
        }
    }
}
