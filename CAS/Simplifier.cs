using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{

    public class PolynomialTerm
    {
        public string Name {  get; set; }
        public double Coeff { get; set; }
        public double Order { get; set; }
    }

    public class AdditionTerm
    {
        public ExprNode Expression {  get; set; }
        public bool Negative { get; set; }
    }
    public class MultiplicationTerm
    {
        public ExprNode Expression { get; set; }
        public bool Reciprocal { get; set; }
    }
    public class Simplifier
    {

        public static List<AdditionTerm> FlattenAddition(ExprNode expression)
        {
            if (expression == null) return null;
            if (expression is AtomNode)
            {
                return new List<AdditionTerm>
                    {
                        new AdditionTerm { Expression = expression, Negative = false },
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
                        new AdditionTerm { Expression = expression , Negative = false},
                    };
            }
            else
                return new List<AdditionTerm>
                    {
                        new AdditionTerm { Expression = expression, Negative = false },
                    };
        }

        public List<MultiplicationTerm> FlattenMultiplication(ExprNode expression)
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
    }
}
