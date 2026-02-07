using CAS.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public class SessionManager
    {
        public static SessionManager Default { get; private set; } = new SessionManager();
        ConstantManager constantManager = new ConstantManager();
        UserVariableManager userVariableManager = new UserVariableManager();
        NativeFunctionManager nativeFunctionManager = new NativeFunctionManager();
        UserFunctionManager userFunctionManager = new UserFunctionManager();

        public ConstantManager ConstantManager { get { return constantManager; } }
        public UserVariableManager UserVariableManager { get {return userVariableManager; } }
        public NativeFunctionManager NativeFunctionManager { get { return nativeFunctionManager; } }
        public UserFunctionManager UserFunctionManager { get { return userFunctionManager; } }

        public IVariable GetVariable(string name)
        {
            if (SessionManager.Default.ConstantManager.VariableExists(name))
            {
                return SessionManager.Default.ConstantManager.GetVariable(name);
            }
            else if (SessionManager.Default.UserVariableManager.VariableExists(name))
            {
                return SessionManager.Default.UserVariableManager.GetVariable(name);
            }
            else
                return null;
        }

        public void SetVariable(string name, ExprNode value)
        {

            if (SessionManager.Default.ConstantManager.VariableExists(name))
                throw new EvaluatorException($"Cannot set variable '{name}' because it refers to a constant");

            var variable = new UserVariable
            {
                Name = name,
                Value = value
            };

            SessionManager.Default.UserVariableManager.SetVariable(name, variable);
        }

        public IFunction GetFunction(string name)
        {
            if (SessionManager.Default.NativeFunctionManager.FunctionExists(name))
            {
                return SessionManager.Default.NativeFunctionManager.GetFunction(name);
            }
            else
                return null;
        }

        public void SetFunction(string name, IFunction function)
        {
            if (SessionManager.Default.NativeFunctionManager.FunctionExists(name))
                throw new EvaluatorException($"Cannot set function '{name}' because it refers to a native function.");
        }
    }
}
