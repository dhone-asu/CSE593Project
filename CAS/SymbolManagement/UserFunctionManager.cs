using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public class UserFunction: IFunction
    {
        public static object Eval()
        {
            return null;
        }
        public string Name { get; set; }
        public GenericFunction Delegate { get; set; }

        public ExprNode Expression { get; set; }

        public ValueType[] Parameters { get; set; }
        public ValueType ReturnValue { get; set; }

        public bool CheckParameterTypes(object[] parameters)
        {
            return false;
        }

        public object Call(object[] parameters)
        {
            return null; 
        }
    }
    public class UserFunctionManager: IFunctionManager
    {
        public Dictionary<string, UserFunction> UserFunctions { get; set; } = new Dictionary<string, UserFunction>();


        public IFunction GetFunction(string name)
        {
            return UserFunctions[name];
        }

        public void SetFunction(string name, UserFunction function)
        {
            UserFunctions[name] = function;
        }

        public bool FunctionExists(string name)
        {
            return UserFunctions.ContainsKey(name);
        }

    }
}
