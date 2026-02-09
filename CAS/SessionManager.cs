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
            if (ConstantManager.VariableExists(name))
            {
                return ConstantManager.GetVariable(name);
            }
            else if (UserVariableManager.VariableExists(name))
            {
                return UserVariableManager.GetVariable(name);
            }
            else
                return null;
        }

        public void SetVariable(string name, ExprNode value)
        {

            if (ConstantManager.VariableExists(name))
                throw new EvaluatorException($"Cannot set variable '{name}' because it refers to a constant");

            var variable = new UserVariable
            {
                Name = name,
                Value = value
            };

            UserVariableManager.SetVariable(name, variable);
        }

        public bool VariableExists(string name)
        {
            return ConstantManager.VariableExists(name) || UserVariableManager.VariableExists(name);
        }

        public IFunction GetFunction(string name)
        {
            if (SessionManager.Default.NativeFunctionManager.FunctionExists(name))
            {
                return NativeFunctionManager.GetFunction(name);
            }
            else
            {
                return UserFunctionManager.GetFunction(name);
            }
        }

        public void SetFunction(string name, IFunction function)
        {
            if (NativeFunctionManager.FunctionExists(name))
                throw new EvaluatorException($"Cannot set function '{name}' because it refers to a native function.");
            if (!(function is UserFunction))
                throw new EvaluatorException($"Only UserFunction can be set");
            UserFunctionManager.SetFunction(name, (UserFunction)function);
        }

        public bool FunctionExists(string name)
        {
            return NativeFunctionManager.FunctionExists(name) || UserFunctionManager.FunctionExists(name);
        }
    }
}
