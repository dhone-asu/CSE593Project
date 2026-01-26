using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public enum AtomType { Numeral, Identifier }
    public class ExprNode { }
    public class OpNode: ExprNode
    {
        public Operator Operator { get; set; }
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
    }

    public class FunctionNode : ExprNode
    {
        public string Name { get; set; }
        public List<ExprNode> Params { get; set; } 
    }

    public class AtomNode : ExprNode
    {        
        public string Data { get; set; }
        public AtomType Type { get; set; }
    }
}
