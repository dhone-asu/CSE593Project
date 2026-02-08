using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAS;

namespace CAS.SymbolManagement
{
    public class UserFunction: IFunction
    {
        public static object Eval()
        {
            return null;
        }
        public string Name { get; set; }
        public ExprNode Expression { get; set; }

        public string[] ParameterNames { get; set; }
        public ParamType[] ParameterTypes { get; set; }
        public ParamType ReturnType { get; set; }

        public bool CheckParameterTypes(object[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (!(parameter is ExprNode))
                    return false;
            }
            return true;
        }

        private object[] GetSubstitutionArray(object[] parameters)
        {
            var args = new List<object>();

            for (int i = 0; i < parameters.Length; i++)
            {
                args.Add(ParameterNames[i]);
                args.Add(parameters[i]);
            }

            return args.ToArray();
        }

        public object Call(object[] parameters)
        {
            var args = GetSubstitutionArray(parameters);

            var afterSubst = Evaluator.Substitute(Expression, args);
            var afterEval = Evaluator.Evaluate(afterSubst);
            return afterEval;
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
