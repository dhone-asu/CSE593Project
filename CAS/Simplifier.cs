using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{

    public class Variable
    {
        public string Name {  get; set; }
        public double Coeff { get; set; }
        public double Order { get; set; }
    }
    public class Simplifier
    {

        public static List<ExprNode> Flatten(ExprNode expression, int prec)
        {
            if (expression == null) return null;
            if (expression is AtomNode)
            {
                return new List<ExprNode> { expression };
            }
            else if (expression is OpNode)
            {
                OpNode opNode = (OpNode)expression;
                if (Parser.PrecedenceMap[opNode.Operator] == prec)
                {
                    var left = Flatten(opNode.Left, prec);
                    var right = Flatten(opNode.Right, prec);
                    left.AddRange(right);
                    return left;
                }
                else
                    return new List<ExprNode> { expression };
            }
            else
            {
                return new List<ExprNode> { expression };
            }
        }
    }
}
