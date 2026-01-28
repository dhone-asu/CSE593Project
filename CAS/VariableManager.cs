using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class VariableManager
    {
    }

    public class Variable
    {
        public string Name { get; set; }
        public ExprNode Value { get; set; }
    }
}
