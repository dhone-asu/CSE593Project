using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public enum AtomType { Numeral, Identifier }
    public class ExprNode: ICloneable
    {
        public virtual object Clone()
        {
            return new ExprNode();
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
                Left = (ExprNode)Left.Clone(),
                Right = (ExprNode)Right.Clone(),
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
