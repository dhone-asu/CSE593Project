using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public enum AtomType { Numeral, Identifier }


    public class ExprNode : ICloneable
    {
        public static readonly AtomNode One = new AtomNode
        {
            Data = 1,
            Type = AtomType.Numeral
        };

        public static readonly AtomNode Zero = new AtomNode
        {
            Data = 0,
            Type = AtomType.Numeral
        };

        public virtual object Clone()
        {
            return new ExprNode();
        }

        public static ExprNode operator ^(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Pow,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator -(ExprNode node)
        {
            return new OpNode
            {
                Operator = Operator.Neg,
                Left = node
            };
        }

        public static ExprNode operator/(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Div,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator *(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Mult,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator+(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Plus,
                Left = a,
                Right = b
            };
        }

        public static ExprNode operator-(ExprNode a, ExprNode b)
        {
            return new OpNode
            {
                Operator = Operator.Plus,
                Left = a,
                Right = b
            };
        }

            
    }
    public class OpNode: ExprNode
    {
        public Operator Operator { get; set; }
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }

        public override object Clone()
        {
            return new OpNode
            {
                Operator = Operator,
                Left = (ExprNode)Left?.Clone(),
                Right = (ExprNode)Right?.Clone(),
            };
        }
    }

    public class FunctionNode : ExprNode
    {
        public string Name { get; set; }
        public List<ExprNode> Params { get; set; }

        public override object Clone()
        {
            var newParams = new List<ExprNode>();

            foreach (var param in Params)
            {
                newParams.Add((ExprNode)param.Clone());
            }

            return new FunctionNode
            {
                Name = Name,
                Params = newParams
            };
        }
    }

    public class AtomNode : ExprNode
    {        
        public object Data { get; set; }
        public AtomType Type { get; set; }

        public override object Clone()
        {
            return new AtomNode
            {
                Data = Data,
                Type = Type
            };
        }
    }
}
