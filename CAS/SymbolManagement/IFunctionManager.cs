using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CAS.SymbolManagement
{
    public delegate object GenericFunction(object[] parameters);
    public enum ParamType { None, Numeral, Variable, Expression, Args };

    public interface IFunction
    {
        string Name { get; set; }
        ParamType ReturnType { get; set; }
        ParamType[] ParameterTypes {  get; set; }
        bool CheckParameterTypes(object[] parameters);
        object Call(object[] parameters);
    }

    public interface IFunctionManager
    {
        IFunction GetFunction(string name);
        bool FunctionExists(string name);
    }
}
