using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{

    public class PolynomialTermComponent
    {
        public ExprNode Base { get; set; }
        public ExprNode Order { get; set; }
    }

    public class PolynomialTerm
    {
        public double Coeff { get; set; }
        public List<PolynomialTermComponent> Components { get; set; } = new List<PolynomialTermComponent>();
    }

    public class Polynomial
    {
        public List<PolynomialTerm> Terms { get; set; } = new List<PolynomialTerm>();
    }

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

        private static PolynomialTermComponent GetPolyTermComponent(ExprNode expression)
        {
            if (expression == null) return null;
            if (expression is AtomNode)
            {
                return new PolynomialTermComponent { Base = expression, Order = ExprNode.One };
            }
            else if (expression is OpNode)
            {
                var opNode = (OpNode)expression;

                if (opNode.Operator == Operator.Pow)
                {
                    return new PolynomialTermComponent { Base = opNode.Left, Order = opNode.Right };
                }
                else
                    return new PolynomialTermComponent { Base = expression, Order = ExprNode.One };
            }
            else
            {
                return new PolynomialTermComponent { Base = expression, Order = ExprNode.One };
            }
        }

        public static Polynomial Polynomialize(List<AdditionTerm> terms)
        {
            var polyList = new List<PolynomialTerm>();

            foreach (var addTerm in terms)
            {
                var polyTerm = new PolynomialTerm { Coeff = 1 };

                foreach (var mulTerm in addTerm.MultiplicationTerms)
                {
                    var polyTermComponent = GetPolyTermComponent(mulTerm.Expression);
                    if (mulTerm.Reciprocal)
                        polyTermComponent.Order = -polyTermComponent.Order;
                    polyTerm.Components.Add(polyTermComponent);
                }
                polyList.Add(polyTerm);
            }
            return new Polynomial
            {
                Terms = polyList
            };
        }

        private static PolynomialTerm SimplifyComponents(PolynomialTerm term)
        {
            return null;
        }

        private static Polynomial SimplifyTerms(Polynomial poly)
        {
            foreach (var term in poly.Terms)
            {
                //term.Components = SimplifyComponents(term)
            }

            return null;
        }


        public static ExprNode Simplify(ExprNode expression)
        {
            var terms = Flatten(expression);

            return null;
        }
    }
}
