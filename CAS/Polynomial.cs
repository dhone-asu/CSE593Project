using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class MonomialFactor : IEquatable<MonomialFactor>
    {
        public ExprNode Base { get; set; }
        public ExprNode Order { get; set; }

        public bool Equals(MonomialFactor a)
        {
            return Base == a.Base && Order == a.Order;
        }

        public ExprNode ToExpression()
        {
            ExprNode factor = null;

            if (Order == ExprNode.One)
                factor = Base;
            else
                factor = Base ^ Order;

            return factor;
        }

        public static MonomialFactor operator *(MonomialFactor a, MonomialFactor b)
        {
            if (a.Base == b.Base)
            {
                var newFactor = new MonomialFactor
                {
                    Base = a.Base,
                    Order = Simplifier.Simplify(a.Order + b.Order)
                };

                return newFactor;
            }
            else
            {
                var newFactor = new MonomialFactor
                {
                    Base = (a.Base ^ a.Order) * (b.Base ^ b.Order),
                    Order = ExprNode.One
                };

                return newFactor;
            }
        }
    }

    public class Monomial : IEquatable<Monomial>
    {
        public double Coeff { get; set; }
        public List<MonomialFactor> Factors { get; set; } = new List<MonomialFactor>();

        public bool FactorsMatch(Monomial a)
        {
            if (Factors.Count != a.Factors.Count)
                return false;

            var factors = new List<MonomialFactor>(Factors);

            foreach (var factor in a.Factors)
            {
                var item = factors.Find(x => x.Equals(factor));

                if (item != null)
                    factors.Remove(item);
            }
            return (factors.Count == 0);
        }
        public bool Equals(Monomial a)
        {
            return this.Coeff == a.Coeff && FactorsMatch(a);
        }

        public ExprNode ToExpression()
        {
            ExprNode newTerm = null;
            if (Math.Abs(Coeff) != 1.0 || Factors.Count() == 0)
                newTerm = new AtomNode { Data = Math.Abs(Coeff), Type = AtomType.Numeral };
            foreach (var factor in Factors)
            {
                if (newTerm == null)
                    newTerm = factor.ToExpression();
                else
                    newTerm = newTerm * factor.ToExpression();
            }
            return newTerm;
        }
    }

    public class Polynomial
    {
        public List<Monomial> Terms { get; set; } = new List<Monomial>();

        public ExprNode ToExpression()
        {
            if (Terms.Count == 0)
                return ExprNode.Zero;

            ExprNode result = null;

            foreach (var term in Terms)
            {
                ExprNode newTerm = term.ToExpression();

                if (result == null)
                {
                    if (term.Coeff >= 0)
                        result = newTerm;
                    else
                        result = -newTerm;
                }
                else
                {
                    if (term.Coeff >= 0)
                        result = result + newTerm;
                    else
                        result = result - newTerm;
                }
            }
            return result;
        }
    }
}
