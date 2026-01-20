using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    /*public enum TokenType
    {
        Numeral, Identifier,
        Plus, Minus, Div, Mult, Pow, Mod,
        Eq, Neq, Gt, Lt, Gte, Lte,
        Lpar, Rpar, Comma,
        Assign,
        Error,
        EndOfInput
    }*/
    public enum BinaryOperator { None, Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte }
    public enum UnaryOperator { Neg }

    public enum AtomType { Numeral, Identifier, }
    public class ExprNode
    {
        public static readonly TokenType ExprOperators = TokenType.Plus | TokenType.Minus | TokenType.Div | 
            TokenType.Mult | TokenType.Pow | TokenType.Mod | TokenType.Eq | TokenType.Neq | TokenType.Gt | 
            TokenType.Lt | TokenType.Gte | TokenType.Lte;

        public string Name { get; set; }
    }
    public class BinaryOpNode: ExprNode
    {
        public BinaryOperator Operator { get; set; }
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
    }

    public class UnaryOpNode: ExprNode
    {
        public UnaryOperator Operator { get; set; }

        public ExprNode Operand { get; set; }

    }

    public class FunctionNode : ExprNode
    {
        public ExprNode[] Params { get; set; } 
    }

    public class AtomNode : ExprNode
    {       
        public AtomType Type { get; set; }
    }
}
