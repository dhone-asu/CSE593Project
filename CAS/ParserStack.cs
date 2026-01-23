using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class ParserStack
    {
        public Stack<StackNode> Stack {  get; set; } = new Stack<StackNode>();
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
