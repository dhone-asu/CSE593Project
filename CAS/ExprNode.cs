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
    public enum Operator { Plus, Minus, Div, Mult, Pow, Mod, Eq, Neq, Gt, Lt, Gte, Lte, Neg, Func }
    public interface IExprNode
    {
    }
    public class BinaryOpNode: IExprNode
    {

    }

    public class UnaryOpNode: IExprNode
    {

    }

    public class FunctionNode : IExprNode
    {

    }

    public class TermNode : IExprNode 
    {
    }
}
