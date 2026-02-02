using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public interface IVariableManager
    {
        IVariable GetVariable(string name);
        bool VariableExists(string name);
    }

    public interface IVariable
    {
        string Name { get; }
        ExprNode Value { get; }
    }
}
