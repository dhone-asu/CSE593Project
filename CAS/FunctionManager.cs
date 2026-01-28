using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public delegate object CasFunction(object[] parameters);
    public interface IFunctionManager
    {
        string[] GetFunctionNames();
        CasFunction[] GetFunctions();
        CasFunction GetFunction(string name);
        bool FunctionExists(string name);
        object CallFunction(string name, object[] parameters);
    }

    public class NativeFunctionManager: IFunctionManager
    {
        private List<CasFunction> functions = new List<CasFunction>();

        public NativeFunctionManager()
        {

        }
        
        public string[] GetFunctionNames()
        {
            return null;
        }

        public CasFunction[] GetFunctions()
        {
            return null; 
        }

        public CasFunction GetFunction(String name)
        {
            return null;
        }

        public bool FunctionExists(string name)
        {
            return false; 
        }

        public object CallFunction(string name, object[] parameters)
        {
            return null;
        }
    }
}
