using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAS.SymbolManagement;

namespace CAS
{
    public class Evaluator
    {
        ConstantManager constantManager = new ConstantManager();
        UserVariableManager variableManager = new UserVariableManager();
        NativeFunctionManager nativeFunctionManager = new NativeFunctionManager();
        UserFunctionManager userFunctionManager = new UserFunctionManager();
        public Evaluator() { }
    }
}
