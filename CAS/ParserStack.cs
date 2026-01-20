using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class ParserStack
    {
    }

    public class StackNode { }

    public class ExprStackNode: StackNode 
    {
        public ExprNode Expr {  get; set; }
    }

    public class TerminalStackNode: StackNode
    {
        public Token Terminal { get; set; }
    }
}
