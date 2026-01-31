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
        VariableManager variableManager = new VariableManager();
        NativeFunctionManager nativeFunctionManager = new NativeFunctionManager();
        public Evaluator() { }
    }
}
