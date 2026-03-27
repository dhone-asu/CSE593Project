using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CAS
{
    public class MonomialFactor : IEquatable<MonomialFactor>
    {
        public bool Negative { get; set; }
        public ExprNode Base { get; set; }
        public ExprNode Order { get; set; }

        public static readonly MonomialFactor One = 
            new MonomialFactor { Base = ExprNode.One, Order = ExprNode.One };

        public static readonly MonomialFactor Zero =
            new MonomialFactor { Base = ExprNode.Zero, Order = ExprNode.One };

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

            if (Negative)
                factor = -factor;

            return factor;
        }

        public List<string> GetVariableNames()
        {
            var list = new List<string>();
            list.AddRange(Base.GetVariableNames());
            list.AddRange(Order.GetVariableNames());

            return list;
        }

        public Monomial Derivative(string variable)
        {
            var baseVars = Base.GetVariableNames();
            var orderVars = Order.GetVariableNames();
            bool baseVarFx = baseVars.Contains(variable);
            bool orderVarFx = orderVars.Contains(variable);

            if (baseVarFx && orderVarFx)
                return MixedRuleDeriv(variable);
            else if (baseVarFx && !orderVarFx)
                return PowerRuleDeriv(variable);
            else if (orderVarFx && !baseVarFx)
                return ExpRuleDeriv(variable);
            else
                return new Monomial
                {
                    Coeff = 0
                };
        }

        private Monomial PowerRuleDeriv(string variable)
        {
            // d/dx (x^n) = n*x^(n-1)
            if (Base is AtomNode && ((AtomNode)Base).Type == AtomType.Identifier)
            {
                var coeff = new MonomialFactor
                {
                    Negative = Negative,
                    Base = Order,
                    Order = ExprNode.One
                };
                var expr = new MonomialFactor
                {
                    Base = Base,
                    Order = Simplification.Resolve(Order - ExprNode.One)
                };
                var mono = new Monomial { Coeff = 1 };
                mono.Factors.Add(coeff);
                mono.Factors.Add(expr);
                return Simplification.SimplifyFactors(mono);
            }
            // d/dx (f(x))^n = f'(x)*n*(f(x))^(n-1)
            else
            {
                //var basePoly = Simplification.Polynomialize(Base);
                //var baseDeriv = basePoly.Derivative(variable);
                Polynomial baseDeriv;

                if (Base is FunctionNode)
                {
                    var baseDerivExpr = ((FunctionNode)Base).Derivative(variable);
                    baseDeriv = Simplification.Polynomialize(baseDerivExpr);
                }
                else
                {
                    var basePoly = Simplification.Polynomialize(Base);
                    baseDeriv = basePoly.Derivative(variable);
                }

                var fprime = new MonomialFactor
                {
                    Negative = Negative,
                    Base = baseDeriv.ToExpression(),
                    Order = ExprNode.One
                };
                var coeff = new MonomialFactor
                {
                    Base = Order,
                    Order = ExprNode.One
                };
                var expr = new MonomialFactor
                {
                    Base = Base,
                    Order = Simplification.Resolve(Order - ExprNode.One)
                };
                var mono = new Monomial { Coeff = 1 };
                mono.Factors.Add(fprime);
                mono.Factors.Add(coeff);
                mono.Factors.Add(expr);
                return Simplification.SimplifyFactors(mono);
            }
        }

        private Monomial ExpRuleDeriv(string variable)
        {
            // d/dx a^x = a^x*log(a)
            if (Order is AtomNode && ((AtomNode)Order).Type == AtomType.Identifier)
            {
                var logBase = new MonomialFactor
                {
                    Base = new FunctionNode
                    {
                        Name = "log",
                        Params = { Base }
                    },
                    Order = ExprNode.One
                };
                var mono = new Monomial { Coeff = 1 };
                mono.Factors.Add(this);
                mono.Factors.Add(logBase);
                return Simplification.SimplifyFactors(mono);
            }
            // d/dx a^f(x) = f'(x)*a^f(x)*log(a)
            else
            {
                Polynomial orderDeriv;

                if (Order is FunctionNode)
                {
                    var derivExpr = ((FunctionNode)Order).Derivative(variable);
                    orderDeriv = Simplification.Polynomialize(derivExpr);
                }
                else
                {
                    var orderPoly = Simplification.Polynomialize(Order);
                    orderDeriv = orderPoly.Derivative(variable);
                }
            
                var orderDerivFactor = new MonomialFactor
                {
                    Base = orderDeriv.ToExpression(),
                    Order = ExprNode.One
                };
                var logBase = new MonomialFactor
                {
                    Base = new FunctionNode
                    {
                        Name = "log",
                        Params = { Base }
                    },
                    Order = ExprNode.One
                };
                var mono = new Monomial { Coeff = 1 };
                mono.Factors.Add(orderDerivFactor);
                mono.Factors.Add(this);
                mono.Factors.Add(logBase);

                return Simplification.SimplifyFactors(mono);
            }
        }


        private Monomial MixedRuleDeriv(string variable)
        {
            // d/dx x^x = x^x*(log(x) + 1)
            if (Base is AtomNode && ((AtomNode)Base).Type == AtomType.Identifier &&
                Order is AtomNode && ((AtomNode)Order).Type == AtomType.Identifier)
            {
                var logxp1 = new MonomialFactor
                {
                    Base = new OpNode
                    {
                        Operator = Operator.Plus,
                        Left = new FunctionNode
                        {
                            Name = "log",
                            Params = { Base }
                        },
                        Right = ExprNode.One
                    },
                    Order = ExprNode.One
                };
                var mono = new Monomial
                {
                    Coeff = 1,
                    Factors = { this, logxp1 }
                };

                return mono;
            }
            // d/dx (f(x))^g(x) = (g'(x)*log(f(x)) + f'(x)*g(x)/f(x))*(f(x))^g(x)
            else
            {
                Polynomial baseDeriv;
                Polynomial orderDeriv;

                if (Base is FunctionNode)
                {
                    var derivExpr = ((FunctionNode)Base).Derivative(variable);
                    baseDeriv = Simplification.Polynomialize(derivExpr);
                }
                else
                {
                    var basePoly = Simplification.Polynomialize(Base);
                    baseDeriv = basePoly.Derivative(variable);
                }

                if (Order is FunctionNode)
                {
                    var derivExpr = ((FunctionNode)Order).Derivative(variable);
                    orderDeriv = Simplification.Polynomialize(derivExpr);
                }
                else
                {
                    var orderPoly = Simplification.Polynomialize(Order);
                    orderDeriv = orderPoly.Derivative(variable);
                }


                var coeffExpr = new OpNode
                {
                    Operator = Operator.Plus,
                    Left = new OpNode
                    {
                        Operator = Operator.Mult,
                        Left = orderDeriv.ToExpression(),
                        Right = new FunctionNode
                        {
                            Name = "log",
                            Params = { Base }
                        }
                    },
                    Right = new OpNode
                    {
                        Operator = Operator.Div,
                        Left = new OpNode
                        {
                            Operator = Operator.Mult,
                            Left = baseDeriv.ToExpression(),
                            Right = Order
                        },
                        Right = Base
                    }
                };
                var coeff = new MonomialFactor
                {
                    Base = coeffExpr,
                    Order = ExprNode.One
                };
                var mono = new Monomial { Coeff = 1 };
                mono.Factors.Add(coeff);
                mono.Factors.Add(this);

                return Simplification.SimplifyFactors(mono);
            }
        }

        public static MonomialFactor operator *(MonomialFactor a, MonomialFactor b)
        {
            if (a.Base == b.Base)
            {
                var newFactor = new MonomialFactor
                {
                    Base = a.Base,
                    Order = Simplification.Simplify(a.Order + b.Order)
                };

                if (newFactor.Order == ExprNode.Zero)
                    newFactor = new MonomialFactor { Base = ExprNode.One, Order = ExprNode.One };

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

        public List<string> GetVariableNames()
        {
            var list = new List<string>();

            foreach (var factor in Factors)
            {
                list.AddRange(factor.GetVariableNames());
            }

            return list;
        }
        
        public Polynomial ToPolynomial()
        {
            var poly = new Polynomial
            {
                Terms = { this }
            };
            return poly;
        }

        public Polynomial Derivative(string variable)
        {
            var poly = new Polynomial();

            for (int i = 0; i < Factors.Count; i++)
            {
                var mono = new Monomial { Coeff = Coeff };

                var factorDerivPoly = Factors[i].Derivative(variable).ToPolynomial();

                var factorderiv = new MonomialFactor
                {
                    Base = factorDerivPoly.ToExpression(),
                    Order = ExprNode.One
                };

                for (int j = 0; j < Factors.Count; j++)
                {
                    if (i == j)
                        mono.Factors.Add(factorderiv);
                    else
                        mono.Factors.Add(Factors[j]);
                }

                poly.Terms.Add(mono);
            }

            return Simplification.Simplify(poly);
        }

        public static Monomial operator*(Monomial a, Monomial b)
        {
            var result = new Monomial();
            result.Coeff = a.Coeff * b.Coeff;
            result.Factors.AddRange(a.Factors);
            result.Factors.AddRange(b.Factors);
            result = Simplification.SimplifyFactors(result);
            return result;
        }

        public static Monomial operator-(Monomial mono)
        {
            var result = new Monomial();
            result.Coeff = -mono.Coeff;
            result.Factors.AddRange(mono.Factors);

            return result;
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

        public List<string> GetVariableNames()
        {
            var list = new List<string>();

            foreach (var term in Terms)
            {
                list.AddRange(term.GetVariableNames());
            }

            return list;
        }

        public Polynomial Derivative(string variable)
        {
            var poly = new Polynomial();

            foreach (var term in Terms)
            {
                poly = poly + term.Derivative(variable);
            }

            return Simplification.Simplify(poly);
        }

        public double ConstantPart()
        {
            double result = 0;

            foreach (var term in Terms)
            {
                if (term.Factors.Count == 0)
                    result += term.Coeff;
            }

            return result;
        }

        public Polynomial VariablePart()
        {
            var result = new Polynomial();

            foreach (var term in Terms)
            {
                if (term.Factors.Any())
                    result.Terms.Add(term);
            }

            return result;
        }

        public static Polynomial operator+(Polynomial a, Polynomial b)
        {
            var result = new Polynomial();
            result.Terms.AddRange(a.Terms);
            result.Terms.AddRange(b.Terms);
            result = Simplification.SimplifyTerms(result);
            return result;
        }

        public static Polynomial operator*(Polynomial a, Polynomial b)
        {
            var poly = new Polynomial();

            foreach (var aTerm in a.Terms)
            {
                foreach (var bTerm in b.Terms)
                {
                    poly.Terms.Add(aTerm * bTerm);
                }
            }

            poly = Simplification.SimplifyTerms(poly);

            return poly;
        }

        public static Polynomial operator^(Polynomial a, uint b)
        {
            if (b == 0)
                return new Polynomial
                {
                    Terms = new List<Monomial>
                    {
                        new Monomial
                        {
                            Coeff = 1
                        }
                    }
                };

            Polynomial poly = null;

            for (int i = 0; i < b; i++)
            {  
                if (poly == null)
                {
                    poly = a;
                }
                else
                {
                    poly = poly * a;
                }
            }

            poly = Simplification.SimplifyTerms(poly);

            return poly;
        }

        public static ExprNode operator^(Polynomial a, double b)
        {
            bool recip = b < 0;
            uint powerInt = (uint)Math.Abs(b - b % 1.0);
            double powerFrac = b % 1.0;
            AtomNode powerFracNode = new AtomNode
            {
                Data = powerFrac,
                Type = AtomType.Numeral
            };

            var expansion = (a ^ powerInt).ToExpression();
            var radicalPart = a.ToExpression() ^ powerFracNode;

            if (recip)
            {
                expansion ^= ExprNode.NegativeOne;
            }

            if (powerFrac == 0)
                return expansion;
            else
                return expansion * radicalPart;
        }

        public static ExprNode operator^(Polynomial a, Polynomial b)
        {
            b = Simplification.SimplifyTerms(b);
            double constPart = b.ConstantPart();
            Polynomial varPart = b.VariablePart();
            ExprNode vpartResult = a.ToExpression() ^ varPart.ToExpression();
            ExprNode cpartResult = a ^ constPart;

            //var result = Simplification.Simplify(vpartResult * cpartResult);
            var result = vpartResult * cpartResult;

            return result;
        }

        public static Polynomial operator-(Polynomial poly)
        {
            var newPoly = new Polynomial();

            foreach (var term in poly.Terms)
            {
                newPoly.Terms.Add(-term);
            }

            return newPoly;
        }

        public static Polynomial operator-(Polynomial a, Polynomial b)
        {
            return a + -b;
        }
    }
}
