using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class AdditionTerm
    {
        public ExprNode Expression {  get; set; }
        public bool Negative { get; set; }
        public List<MultiplicationTerm> MultiplicationTerms { get; set; }
    }
    public class MultiplicationTerm
    {
        public ExprNode Expression { get; set; }
        public bool Reciprocal { get; set; }
    }
    public class Simplifier
    {

        private static List<AdditionTerm> FlattenAddition(ExprNode expression)
        {
            if (expression == null) return null;
            if (expression is AtomNode)
            {
                return new List<AdditionTerm>
                {
                    new AdditionTerm { Expression = expression, Negative = false }
                };
            }
            else if (expression is OpNode)
            {
                OpNode opNode = (OpNode)expression;
                if (opNode.Operator == Operator.Plus || opNode.Operator == Operator.Minus)
                {
                    var left = FlattenAddition(opNode.Left);
                    var right = FlattenAddition(opNode.Right);
                    if (opNode.Operator == Operator.Minus)
                    {
                        foreach (var item in right)
                        {
                            item.Negative = !item.Negative;
                        }
                    }
                    left.AddRange(right);
                    return left;
                }
                else if (opNode.Operator == Operator.Neg)
                {
                    var left = FlattenAddition(opNode.Left);

                    foreach (var item in left)
                    {
                        item.Negative = !item.Negative;
                    }

                    return left;
                }
                else
                    return new List<AdditionTerm>
                    {
                        new AdditionTerm { Expression = expression , Negative = false}
                    };
            }
            else
                return new List<AdditionTerm>
                {
                    new AdditionTerm { Expression = expression, Negative = false }
                };
        }

        private static List<MultiplicationTerm> FlattenMultiplication(ExprNode expression)
        {
            if (expression == null) return null;

            if (expression is AtomNode)
            {
                return new List<MultiplicationTerm>()
                {
                    new MultiplicationTerm { Expression = expression, Reciprocal = false }
                };
            }
            else if (expression is OpNode)
            {
                var opNode = (OpNode)expression;

                if (opNode.Operator == Operator.Mult || opNode.Operator == Operator.Div)
                {
                    var left = FlattenMultiplication(opNode.Left);
                    var right = FlattenMultiplication(opNode.Right);

                    if (opNode.Operator == Operator.Div)
                    {
                        foreach (var item in right)
                        {
                            item.Reciprocal = !item.Reciprocal;
                        }
                    }
                    left.AddRange(right);
                    return left;
                }
                else
                    return new List<MultiplicationTerm>()
                    {
                        new MultiplicationTerm { Expression = expression, Reciprocal = false }
                    };
            }
            else
                return new List<MultiplicationTerm>()
                {
                    new MultiplicationTerm { Expression = expression, Reciprocal = false }
                };
        }

        public static List<AdditionTerm> Flatten(ExprNode expression)
        {
            var flattenAdd = FlattenAddition(expression);
            
            foreach (var item in flattenAdd)
            {
                item.MultiplicationTerms = FlattenMultiplication(item.Expression);
            }

            return flattenAdd;
        }

        private static MonomialFactor GetFactor(ExprNode expression)
        {
            if (expression == null) return null;
            if (expression is AtomNode)
            {
                return new MonomialFactor { Base = expression, Order = ExprNode.One };
            }
            else if (expression is OpNode)
            {
                var opNode = (OpNode)expression;

                if (opNode.Operator == Operator.Pow)
                {
                    return new MonomialFactor { Base = opNode.Left, Order = opNode.Right };
                }
                else
                    return new MonomialFactor { Base = expression, Order = ExprNode.One };
            }
            else
            {
                return new MonomialFactor { Base = expression, Order = ExprNode.One };
            }
        }

        public static Polynomial Polynomialize(List<AdditionTerm> terms)
        {
            var polyList = new List<Monomial>();

            foreach (var addTerm in terms)
            {
                var mono = new Monomial { Coeff = 1 };
                if (addTerm.Negative)
                    mono.Coeff = -1;

                foreach (var mulTerm in addTerm.MultiplicationTerms)
                {
                    var factor = GetFactor(mulTerm.Expression);
                    if (mulTerm.Reciprocal)
                        factor.Order = -factor.Order;
                    mono.Factors.Add(factor);
                }
                polyList.Add(mono);
            }
            return new Polynomial
            {
                Terms = polyList
            };
        }

        private static Monomial SimplifyFactors(Monomial mono)
        {
            var variables = new List<string>();
            var baseExpressions = new List<ExprNode>();
            var result = new Monomial { Coeff = mono.Coeff };

            
            foreach (var factor in mono.Factors)
            {
                factor.Base = Simplify(factor.Base);
                factor.Order = Simplify(factor.Order);

                if (factor.Base is AtomNode && factor.Order is AtomNode &&
                    ((AtomNode)factor.Base).Type == AtomType.Numeral &&
                    ((AtomNode)factor.Order).Type == AtomType.Numeral)
                {
                    var baseVal = (double)((AtomNode)factor.Base).Data;
                    var orderVal = (double)((AtomNode)factor.Order).Data;
                    double value = Math.Pow(baseVal, orderVal);
                    result.Coeff *= value;
                }
                else
                {
                    if (!baseExpressions.Contains(factor.Base))
                    {
                        baseExpressions.Add(factor.Base);
                        result.Factors.Add(factor);
                    }
                    else
                    {
                        var existing = result.Factors.Find(x => x.Base == factor.Base);
                        result.Factors.Remove(existing);

                        var newFactor = existing * factor;
                        result.Factors.Add(newFactor);
                    }
                }
            }
            return result;
        }

        private static Polynomial SimplifyTerms(Polynomial poly)
        {
            var simplified = new Polynomial();

            foreach (var mono in poly.Terms)
            {
                var newTerm = SimplifyFactors(mono);
                if (newTerm.Coeff != 0)
                    simplified.Terms.Add(newTerm);
            }

            var result = new Polynomial();

            foreach (var mono in simplified.Terms)
            {
                var match = result.Terms.Find(x => mono.FactorsMatch(x));

                if (match == null)
                    result.Terms.Add(mono);
                else
                {
                    match.Coeff += mono.Coeff;
                }
            }

            result.Terms.RemoveAll(x => x.Coeff == 0); // Remove any coeffs that became zero
            return result;
        }

        public static FunctionNode SimplifyFunction(FunctionNode node)
        {
            var result = new FunctionNode { Name = node.Name };

            foreach (var param in node.Params)
            {
                result.Params.Add(Simplify(param));
            }

            return result;
        }


        public static ExprNode Simplify(ExprNode expression)
        {
            if (expression is AtomNode)
                return expression;
            else if (expression is FunctionNode)
                return SimplifyFunction((FunctionNode)expression);
            else
            {
                var terms = Flatten(expression);
                var poly = Polynomialize(terms);
                var result = SimplifyTerms(poly);

                return result.ToExpression();
            }
        }
    }
}
