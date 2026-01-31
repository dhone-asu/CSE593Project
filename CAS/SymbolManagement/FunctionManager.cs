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
    public enum ValueType { None, Numeral, Variable, Expression };

    public class ParameterException: Exception
    {
        public ParameterException(string message) : base(message) { }
        public ParameterException(): base() { }
    }

    public interface IFunction
    {
        string Name { get; set; }
        GenericFunction Delegate { get; set; }
        ValueType ReturnValue { get; set; }
        ValueType[] Parameters {  get; set; }
        bool CheckParameterTypes(object[] parameters);
        object Call(object[] parameters);
    }

    public interface IFunctionManager
    {
        IFunction GetFunction(string name);
        bool FunctionExists(string name);
    }
}
